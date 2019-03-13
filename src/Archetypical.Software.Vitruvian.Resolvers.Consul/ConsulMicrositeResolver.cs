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

                return null;
            }
        }

        public Task<List<Microsite>> GetBySlugAsync(string slug)
        {
            throw new System.NotImplementedException();
        }

        public async Task<(Microsite microsite, bool success, string message)> RegisterAsync(Microsite microSite)
        {
            using (var consul = new ConsulClient(_configuration))
            {
                var dcs = consul.Catalog.Datacenters();
                var nodes = consul.Catalog.Nodes();
                await Task.WhenAll(dcs, nodes);
                var dc = DataCenterResolver(dcs.Result.Response, microSite);
                var node = NodeResolver(nodes.Result.Response, microSite);
                var tags = microSite.Tags.ToList();
                tags.Add("Vitruvian");
                tags.Add("Microsite");
                var address = microSite.Endpoints.First();
                var serviceName = $"{microSite.Name} - microsite";
                var service = new AgentService()
                {
                    ID = serviceName,
                    Service = "Vitruvian",
                    Tags = tags.ToArray(),
                    Port = address.Uri.Port
                };

                var check = new AgentCheck()
                {
                    Node = node.Name,
                    CheckID = $"vitruvian:{serviceName}",
                    Name = "Microsite Health check",
                    Notes = "Script based health check",
                    Status = HealthStatus.Passing,
                    ServiceID = serviceName
                };

                var registration = new CatalogRegistration()
                {
                    Datacenter = dc,
                    Node = node.Name,
                    Address = new Uri(address.Uri, "/microsite/probe").ToString(),
                    Service = service,
                    Check = check
                };
                var result = await consul.Catalog.Register(registration);
                return (microSite, result.StatusCode == System.Net.HttpStatusCode.OK, "");
            }
        }

        public Task<(Microsite microsite, bool success, string message)> UnRegisterAsync(Microsite microSite)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSideCar { get; set; }
    }
}