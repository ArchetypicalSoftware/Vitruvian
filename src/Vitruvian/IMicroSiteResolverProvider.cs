using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian
{
    public interface IMicroSiteResolverProvider
    {
        Task<List<MicroSite>> GetAll();

        Task<List<MicroSite>> GetBySlug(string slug);

        bool IsSideCar { get; }
    }
}