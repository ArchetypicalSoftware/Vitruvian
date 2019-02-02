using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Archetypical.Software.Vitruvian
{
    public static class VitruvianExtensions
    {
        private static HttpClientHandler handler = new HttpClientHandler { UseCookies = false };
        private static readonly HttpClient Client = new HttpClient(handler);

        private static readonly List<string> _unsupportedRequestHeaders =
            new List<string>{
                "Transfer-Encoding"
            };

        public static void UseVitruvian(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseVitruvian(cfg => { }, env);
        }

        public static void UseVitruvian(this IApplicationBuilder app, Action<VitruvianConfiguration> cfg, IHostingEnvironment env)
        {
            var configs = new VitruvianConfiguration();
            cfg(configs);
            app.Run(async ctx =>
            {
                var sw = Stopwatch.StartNew();

                var url = new UriBuilder(ctx.Request.Scheme, ctx.Request.Host.Host,
                    ctx.Request.Host.Port.GetValueOrDefault(80), ctx.Request.Path.Value).Uri;
                var slug =
                    url.Segments.FirstOrDefault(x => !x.Equals("/")) ?? "/";

                var newPathSegments = url.Segments.Skip(1);

                var resolver = app.ApplicationServices.GetService<IMicroSiteResolverProvider>();
                var microsite = await resolver.GetBySlug(slug);
                if (microsite.Any())
                {
                    try
                    {
                        var req = new HttpRequestMessage();
                        ctx.Request.Headers.ToList().ForEach(h => req.Headers.Add(h.Key, h.Value.ToString()));
                        var cookieHeader = string.Join("; ", ctx.Request.Cookies.ToList().Select(cookie => $"{cookie.Key}={cookie.Value}"));
                        req.Headers.Add("Cookie", cookieHeader);
                        req.RequestUri = new UriBuilder(microsite.First().Endpoints.First().Uri + string.Join("/", newPathSegments)).Uri;
                        req.Headers.Host = req.RequestUri.Host;
                        var response = await Client.SendAsync(req);
                        response.Headers.ToList().ForEach(h => ctx.Response.Headers.Add(h.Key, string.Join(";", h.Value)));
                        response.Content.Headers.ToList().ForEach(h => ctx.Response.Headers.Add(h.Key, string.Join(";", h.Value)));
                        ctx.Response.StatusCode = (int)response.StatusCode;

                        var otherResponse = await response.Content.ReadAsStreamAsync();
                        _unsupportedRequestHeaders.ForEach(key => ctx.Response.Headers.Remove(key));
                        ctx.Response.Headers.Add("Vitruvian-Elapsed", sw.Elapsed.ToString());
                        ctx.Response.Headers.Add("Vitruvian-Server", Environment.MachineName);
                        ctx.Response.Headers.Add("Vitruvian-Thread", Thread.CurrentThread.ManagedThreadId.ToString());
                        await otherResponse.CopyToAsync(ctx.Response.Body);
                        await ctx.Response.Body.FlushAsync();
                    }
                    catch (Exception e)
                    {
                        ctx.Response.StatusCode = 500;
                        await ctx.Response.WriteAsync(e.ToString());
                    }
                }
                else
                {
                    await ctx.Response.WriteAsync($"Hi there. Your slug is {slug} and your resolver is {resolver}");
                }
            });

            var life = app.ApplicationServices.GetService<IApplicationLifetime>();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);
        }

        public static void AddVitruvian(this IServiceCollection services, Action<VitruvianConfiguration> cfg)
        {
            var config = new VitruvianConfiguration();
            cfg(config);
            services.AddSingleton(config.Resolver);
        }
    }
}