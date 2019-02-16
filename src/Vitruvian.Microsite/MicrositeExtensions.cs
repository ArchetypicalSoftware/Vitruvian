using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public static class MicrositeExtensions
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler { UseCookies = false };
        private static readonly HttpClient Client = new HttpClient(Handler);

        public static void UseAsMicrositeWithVitruvian(this IApplicationBuilder app)
        {
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            var config = app.ApplicationServices.GetService<MicrositeConfiguration>();
            var addresses = app.ApplicationServices.GetService<IServerAddressesFeature>();
            life.ApplicationStarted.Register(() =>
            {
                var req = new HttpRequestMessage();
                req.Method = HttpMethod.Post;
                req.RequestUri = config.VitruvianGatewayUri;
                req.Content = new ObjectContent(typeof(Common.Microsite), config.MicroSite, new JsonMediaTypeFormatter());
                Client.SendAsync(req);
                //publish im coming up
            });
            life.ApplicationStopping.Register(() =>
            {
                //publish im going down
            });

            app.Map("/microsite/probe", apb => { apb.Run(ctx => ctx.Response.WriteAsync("Healthy")); });
        }

        public static void AddVitruvianIntegration(this IServiceCollection services, Action<MicrositeConfiguration> cfg)
        {
            var config = new MicrositeConfiguration();
            cfg(config);
            if (config.Version == new Version())
            {
                config.Version = Assembly.GetCallingAssembly().GetName().Version;
            }

            services.AddSingleton(config);
        }
    }
}