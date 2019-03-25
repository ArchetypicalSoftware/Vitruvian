using Archetypical.Software.Vitruvian.Common;
using Archetypical.Software.Vitruvian.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian.Samples
{
    public class SampleMicrositeResolver : IMicrositeResolver
    {
        private readonly List<Microsite> _sites = new List<Microsite>
        {
            new Microsite("cnn", "CNN", new Version(), new Endpoint(new Uri("https://www.cnn.com/")), "news", "test"),
            new Microsite("archetypical.software", "Archetypical Software", new Version(),
                new Endpoint(new Uri("https://archetypical.software/")), "news", "test"),
            new Microsite("/", "Archetypical Software", new Version(),
                new Endpoint(new Uri("https://archetypical.software/")), "news", "test")
        };

        public Task<List<Microsite>> GetAllAsync()
        {
            return Task.FromResult(_sites);
        }

        public Task<List<Microsite>> GetBySlugAsync(string slug)
        {
            return Task.FromResult(_sites.Where(x => x.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase)).ToList());
        }

        public Task<(Microsite microsite, bool success, string message)> RegisterAsync(Microsite microSite)
        {
            var existing = _sites.Find(x => x.Slug.Equals(microSite.Slug, StringComparison.CurrentCultureIgnoreCase) && x.Endpoint.Uri.Equals(microSite.Endpoint.Uri));
            if (existing == null)
            {
                _sites.Add(microSite);
                return Task.FromResult((microSite, true, "Added new Microsite"));
            }
            else
            {
                return Task.FromResult((existing, true, "Updated existing Microsite"));
            }
        }

        public Task<(Microsite microsite, bool success, string message)> UnRegisterAsync(Microsite microSite)
        {
            var existing = _sites.Find(x => x.Slug.Equals(microSite.Slug, StringComparison.CurrentCultureIgnoreCase) && x.Endpoint.Uri.Equals(microSite.Endpoint.Uri));
            if (existing != null)
            {
                _sites.Remove(existing);
                return Task.FromResult((existing, true, "Fully removed Microsite"));
            }
            return Task.FromResult((microSite, false, "No instance registered for Microsite"));
        }

        public bool IsSideCar { get; } = false;
    }
}