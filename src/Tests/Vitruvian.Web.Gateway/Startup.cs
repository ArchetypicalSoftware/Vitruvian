using Archetypical.Software.Vitruvian.Resolvers.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Archetypical.Software.Vitruvian.Web.Gateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddVitruvian(cfg =>
            {
                cfg.AddMicrositeResolver(new ConsulMicrositeResolver(config =>
                {
                    // This will use the default settings (pointing to local host)
                    //additional config here.
                }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseVitruvian(cfg =>
            {
                //additional config here.
                //Startup/stutdown etc
            }, env);
        }
    }
}