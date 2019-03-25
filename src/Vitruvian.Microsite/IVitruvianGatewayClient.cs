using System.Threading.Tasks;
using Archetypical.Software.Vitruvian.Common.Models.Commands;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public interface IVitruvianGatewayClient
    {
        Task SendCommand(BaseCommand command);
    }
}