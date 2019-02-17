using Newtonsoft.Json;

namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    [JsonConverter(typeof(BaseCommandConverter))]
    public abstract class BaseCommand
    {
        public abstract Command Command { get; }
    }
}