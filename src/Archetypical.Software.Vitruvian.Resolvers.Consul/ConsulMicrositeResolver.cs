using Archetypical.Software.Vitruvian.Common;
using Archetypical.Software.Vitruvian.Common.Interfaces;
using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian.Resolvers.Consul
{
    public class ConsulMicrositeResolver : IMicrositeResolver
    {
        private const string SlugMeta = "Slug";
        private const string NameMeta = "Name";
        private const string VersionMeta = "Version";
        private const string VitruvianTag = "Vitruvian";
        private const string MicrositeTag = "Microsite";
        private const string EndpointUriMeta = "EndpointUri";
        private readonly Action<ConsulClientConfiguration> _configuration;
        private Func<string[], Microsite, string> DataCenterResolver = (dcs, ms) => dcs.First();
        private Func<Node[], Microsite, Node> NodeResolver = (nodes, ms) => nodes.First();

        public ConsulMicrositeResolver(Action<ConsulClientConfiguration> configuration = null)
        {
            if (configuration != null)
            { _configuration = configuration; }
            else { _configuration = x => { }; }
        }

        public async Task<List<Microsite>> GetAllAsync()
        {
            using (var consul = new ConsulClient(_configuration))
            {
                var services = await consul.Catalog.Services();

                var tasks = services.Response.Keys.Select(async k =>
                {
                    using (var innerConsul = new ConsulClient(_configuration))
                    {
                        return await innerConsul.Catalog.Service(k);
                    }
                }).ToList();

                await Task.WhenAll(tasks);
                var results = tasks.Select(x => x.Result).ToList();
                var orderedResponses = results.OrderBy(x => x.LastContact).SelectMany(x => x.Response).ToList();
                var servicesDetailed = orderedResponses.Where(x => x.ServiceTags.Contains(VitruvianTag) && x.ServiceTags.Contains(MicrositeTag)).ToList();
                return servicesDetailed.Select(x =>
                {
                    x.ServiceMeta.TryGetValue(SlugMeta, out string slug);
                    x.ServiceMeta.TryGetValue(NameMeta, out string name);
                    x.ServiceMeta.TryGetValue(EndpointUriMeta, out string endpointUri);
                    var version = new Version("1.0");
                    if (x.ServiceMeta.TryGetValue(VersionMeta, out string versionString))
                    {
                        Version.TryParse(versionString, out version);
                    }
                    return new Microsite(slug, name, version, new Endpoint(new Uri(endpointUri)), x.ServiceTags);
                }).ToList();
            }
        }

        public async Task<List<Microsite>> GetBySlugAsync(string slug)
        {
            return (await GetAllAsync()).Where(x => x.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public async Task<(Microsite microsite, bool success, string message)> RegisterAsync(Microsite microSite)
        {
            using (var consul = new ConsulClient(_configuration))
            {
                var tags = microSite.Tags.ToList();
                tags.Add(VitruvianTag);
                tags.Add(MicrositeTag);
                var address = microSite.Endpoint;
                var serviceName = $"{microSite.Name} - microsite";
                var service = new AgentServiceRegistration()
                {
                    ID = serviceName,
                    Name = serviceName,
                    Tags = tags.Distinct().ToArray(),
                    Port = address.Uri.Port,
                    Meta = new Dictionary<string, string>
                    {
                        {SlugMeta,microSite.Slug },
                        {NameMeta, microSite.Name },
                        {VersionMeta,microSite.Version.ToString() },
                        {EndpointUriMeta,microSite.Endpoint.Uri.ToString() }
                    },
                    Checks = new[]
                    {
                        new AgentServiceCheck
                        {
                            HTTP = new Uri(address.Uri, "/microsite/probe").ToString(),
                            Interval = TimeSpan.FromSeconds(30),
                            DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(3)
                        }
                    }
                };

                var result = await consul.Agent.ServiceRegister(service);
                return (microSite, result.StatusCode == System.Net.HttpStatusCode.OK, "");
            }
        }

        public Task<(Microsite microsite, bool success, string message)> UnRegisterAsync(Microsite microSite)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSideCar { get; } = false;
    }
}