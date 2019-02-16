using Archetypical.Software.Vitruvian.Common;

namespace Archetypical.Software.Vitruvian.Models.Commands
{
    public class AddCommand : BaseCommand
    {
        protected internal override Command Command { get; } = Command.Add;
        public Microsite Microsite { get; set; }
    }
}