using Archetypical.Software.Vitruvian.Common.Interfaces;
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
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian
{
    public static class VitruvianExtensions
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler { UseCookies = false };
        private static readonly HttpClient Client = new HttpClient(Handler);

        private static readonly List<string> UnsupportedRequestHeaders =
            new List<string>{
                "Transfer-Encoding"
            };

        public static void UseVitruvian(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseVitruvian(cfg => { }, env);
        }

        private static (string slug, IEnumerable<string> downstreamPathSegments) ResolveSlug(HttpContext ctx)
        {
            var url = new UriBuilder(ctx.Request.Scheme, ctx.Request.Host.Host, ctx.Request.Host.Port.GetValueOrDefault(80), ctx.Request.Path.Value).Uri;
            var segments = url.Segments.ToList();
            segments.RemoveAll(x => x.Equals("/"));
            var slug = segments.FirstOrDefault() ?? "/";
            slug = slug.Trim('/');
            if (string.IsNullOrWhiteSpace(slug))
            {
                //make this slug the root
                slug = "/";
            }
            var newPathSegments = segments.Count > 1 ? segments.Skip(1) : new List<string>();
            return (slug, newPathSegments);
        }

        private static async Task VitruvianHandler(HttpContext ctx, IServiceProvider applicationServices)
        {
            var sw = Stopwatch.StartNew();

            var slugResult = ResolveSlug(ctx);

            var resolver = applicationServices.GetService<IMicrositeResolver>();
            var microsite = await resolver.GetBySlugAsync(slugResult.slug);
            if (microsite.Any())
            {
                try
                {
                    var req = new HttpRequestMessage();
                    ctx.Request.Headers.ToList().ForEach(h => req.Headers.Add(h.Key, h.Value.ToString()));
                    var cookieHeader = string.Join("; ", ctx.Request.Cookies.ToList().Select(cookie => $"{cookie.Key}={cookie.Value}"));
                    req.Headers.Add("Cookie", cookieHeader);
                    req.RequestUri = new UriBuilder(microsite.First().Endpoint.Uri + string.Join("/", slugResult.downstreamPathSegments)).Uri;
                    req.Headers.Host = req.RequestUri.Host;
                    var response = await Client.SendAsync(req);
                    response.Headers.ToList().ForEach(h => ctx.Response.Headers.Add(h.Key, string.Join(";", h.Value)));
                    response.Content.Headers.ToList().ForEach(h => ctx.Response.Headers.Add(h.Key, string.Join(";", h.Value)));
                    ctx.Response.StatusCode = (int)response.StatusCode;

                    var otherResponse = await response.Content.ReadAsStreamAsync();
                    UnsupportedRequestHeaders.ForEach(key => ctx.Response.Headers.Remove(key));
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
                await ctx.Response.WriteAsync($"Hi there. Your slug is {slugResult.slug} and your resolver is {resolver}");
            }
        }

        public static void UseVitruvian(this IApplicationBuilder app, Action<VitruvianBehaviorConfiguration> cfg, IHostingEnvironment env)
        {
            var configs = new VitruvianBehaviorConfiguration();
            cfg(configs);
            var adminHandler = app.ApplicationServices.GetService<AdministrationHandlers>();
            app.Map("/vitruvian/admin", apb => apb.Run(adminHandler.AdminDelegate));
            app.Run(ctx => VitruvianHandler(ctx, app.ApplicationServices));

            var life = app.ApplicationServices.GetService<IApplicationLifetime>();

            life.ApplicationStarted.Register(() =>
            {
                configs.StartupActions.ForEach(a =>
                {
                    try
                    {
                        a();
                    }
                    catch (Exception e)
                    {
                        //Log
                    }
                });
            });
            life.ApplicationStopping.Register(() =>
            {
                configs.ShutdownActions.ForEach(a =>
                {
                    try
                    {
                        a();
                    }
                    catch (Exception e)
                    {
                        //Log
                    }
                });
            });
        }

        public static void AddVitruvian(this IServiceCollection services, Action<VitruvianConfiguration> cfg)
        {
            var config = new VitruvianConfiguration();
            cfg(config);
            services.AddSingleton(config.Resolver);
            services.AddSingleton<AdministrationHandlers>();
        }

        public static void AddMicrositeResolver(this VitruvianConfiguration cfg, IMicrositeResolver resolver)
        {
            cfg.Resolver = resolver;
        }

        public static void AddStartupAction(this VitruvianBehaviorConfiguration cfg, Action startupAction)
        {
            cfg.StartupActions.Add(startupAction);
        }

        public static void AddShutdownAction(this VitruvianBehaviorConfiguration cfg, Action shutdownAction)
        {
            cfg.StartupActions.Add(shutdownAction);
        }
    }
}