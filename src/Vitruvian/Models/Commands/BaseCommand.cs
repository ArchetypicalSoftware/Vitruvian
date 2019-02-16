using Newtonsoft.Json;

namespace Archetypical.Software.Vitruvian.Models.Commands
{
    [JsonConverter(typeof(BaseCommandConverter))]
    public abstract class BaseCommand
    {
        protected internal abstract Command Command { get; }
    }
}