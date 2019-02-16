using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian.Common.Interfaces
{
    public interface IMicrositeResolver
    {
        Task<List<Microsite>> GetAllAsync();

        Task<List<Microsite>> GetBySlugAsync(string slug);

        Task<(Microsite microsite, bool success, string message)> RegisterAsync(Microsite microSite);

        Task<(Microsite microsite, bool success, string message)> UnRegisterAsync(Microsite microSite);

        bool IsSideCar { get; }
    }

    public interface ILoadBalancer
    {
        Task<Microsite> Balance(string slug);
    }
}