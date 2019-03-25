using Archetypical.Software.Vitruvian.Common.Models.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public static class MicrositeExtensions
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler { UseCookies = false };
        private static readonly HttpClient Client = new HttpClient(Handler);

        public static string GetFullyQualifiedDomainName()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            if (string.IsNullOrWhiteSpace(domainName) || domainName.Equals("(none)" /*This is a docker thing*/))
            {
                return hostName;
            }

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
            {
                hostName += domainName;   // add the domain name part
            }

            return hostName;// return the fully qualified name
        }

        public static void UseAsMicrositeWithVitruvian(this IApplicationBuilder app)
        {
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            var config = app.ApplicationServices.GetService<MicrositeConfiguration>();
            var feature = app.ApplicationServices.GetService<IServer>().Features
                .FirstOrDefault(x => x.Value is IServerAddressesFeature).Value as IServerAddressesFeature;
            var addresses = feature?.Addresses;
            if (addresses != null && addresses.Any())
            {
                config.Endpoint = addresses.Select(x =>
                {
                    var url = new Uri(x.Replace("+:", "localhost:").Replace("::", "localhost:"));
                    if (url.Host.Equals("localhost"))
                    {
                        url = new UriBuilder(url.Scheme, GetFullyQualifiedDomainName(), url.Port, url.PathAndQuery).Uri;
                    }
                    return new Common.Endpoint(url);
                }).FirstOrDefault();
            }

            life.ApplicationStarted.Register(async () =>
            {
                var command = new AddCommand
                {
                    Microsite = config.MicroSite
                };

                await app.ApplicationServices.GetService<IVitruvianGatewayClient>().SendCommand(command);
            });
            life.ApplicationStopping.Register(async () =>
            {
                var command = new DeleteCommand
                {
                    Microsite = config.MicroSite
                };
                await app.ApplicationServices.GetService<IVitruvianGatewayClient>().SendCommand(command);
            });

            app.Map("/microsite/probe", apb => { apb.Run(ctx => ctx.Response.WriteAsync("Healthy")); });
        }

        public static void AddVitruvianIntegration(this IServiceCollection services, Action<MicrositeConfiguration> cfg)
        {
            var config = new MicrositeConfiguration();
            cfg(config);
            if (config.Version == null || config.Version == new Version())
            {
                config.Version = Assembly.GetCallingAssembly().GetName().Version;
            }

            services.AddSingleton<IUrlHelperFactory, MicrositeUrlHelperFactory>();

            services.AddSingleton(config);

            services.AddHttpClient<IVitruvianGatewayClient, VitruvianGatewayClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
            Random jitter = new Random();
            Policy
                .Handle<HttpRequestException>() // etc
                .WaitAndRetry(5,    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitter.Next(0, 100))
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}