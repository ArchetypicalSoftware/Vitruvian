using Archetypical.Software.Vitruvian.Common.Models;
using Archetypical.Software.Vitruvian.Common.Models.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
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
            var feature = app.ApplicationServices.GetService<IServer>().Features
                .FirstOrDefault(x => x.Value is IServerAddressesFeature).Value as IServerAddressesFeature;
            var addresses = feature?.Addresses;
            config.Endpoint = addresses?.Select(x => new Common.Endpoint(new Uri(x))).FirstOrDefault();
            life.ApplicationStarted.Register(async () =>
            {
                var req = new HttpRequestMessage();
                req.Method = HttpMethod.Post;
                req.RequestUri = new Uri(config.VitruvianGatewayUri, "/vitruvian/admin");
                var command = new AddCommand
                {
                    Microsite = config.MicroSite
                };

                req.Content =
                    new StringContent(JsonConvert.SerializeObject(command,
                        SerializerSettings.CommandJsonSerializerSettings), System.Text.Encoding.UTF8, "application/json");
                try
                {
                    var response = await Client.SendAsync(req);
                }
                catch (Exception)
                {
                }
                //publish im coming up
            });
            life.ApplicationStopping.Register(async () =>
            {
                var req = new HttpRequestMessage();
                req.Method = HttpMethod.Post;
                req.RequestUri = new Uri(config.VitruvianGatewayUri, "/vitruvian/admin");
                var command = new DeleteCommand
                {
                    Microsite = config.MicroSite
                };
                req.Content =
                    new StringContent(JsonConvert.SerializeObject(command,
                        SerializerSettings.CommandJsonSerializerSettings), System.Text.Encoding.UTF8, "application/json");

                var response = await Client.SendAsync(req);
                //publish im going down
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
            //TODO: Add the URL helper to prepend the Slug the Links
            //services.AddScoped<IActionContextAccessor, ActionContextAccessor>();
            //services.AddScoped<IUrlHelper>(x =>
            //{
            //    var actionContext = x.GetService<IActionContextAccessor>().ActionContext;
            //    return new MicrositeUrlHelper(actionContext);
            //});
            services.AddSingleton(config);
        }
    }

    public class MicrositeUrlHelper : IUrlHelper
    {
        private ActionContext _actionContext;

        public MicrositeUrlHelper(ActionContext actionContext)
        {
            _actionContext = actionContext;
        }

        public string Action(UrlActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string Content(string contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl(string url)
        {
            throw new NotImplementedException();
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            throw new NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            throw new NotImplementedException();
        }

        public ActionContext ActionContext { get; set; }
    }
}