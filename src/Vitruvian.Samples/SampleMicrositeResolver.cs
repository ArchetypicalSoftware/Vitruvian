using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian.Samples
{
    public class SampleMicrositeResolver : IMicroSiteResolverProvider
    {
        public Task<List<MicroSite>> GetAll()
        {
            return Task.FromResult(new List<MicroSite>
            {
                new MicroSite("cnn","CNN",new Version(),new Endpoint(new Uri("https://www.cnn.com/")),"news","test"),
                new MicroSite("archetypical.software","Archetypical Software",new Version(),new Endpoint(new Uri("https://archetypical.software/")),"news","test")
            });
        }

        public Task<List<MicroSite>> GetBySlug(string slug)
        {
            return Task.FromResult(new List<MicroSite>
            {
                slug.Equals("cnn",StringComparison.CurrentCultureIgnoreCase)? new MicroSite("/cnn","CNN",new Version(),new Endpoint(new Uri("https://www.cnn.com/")),"news","test"):
                    new MicroSite("archetypical.software","Archetypical Software",new Version(),new Endpoint(new Uri("https://archetypical.software/")),"news","test")
            });
        }

        public bool IsSideCar { get; } = false;
    }
}