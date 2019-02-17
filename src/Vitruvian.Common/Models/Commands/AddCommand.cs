namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    public class AddCommand : BaseCommand
    {
        public override Command Command { get; } = Command.Add;
        public Microsite Microsite { get; set; }
    }
}