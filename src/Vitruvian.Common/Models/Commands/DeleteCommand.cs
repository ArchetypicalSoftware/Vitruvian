namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    public class DeleteCommand : BaseCommand
    {
        public override Command Command { get; } = Command.Delete;
        public Microsite Microsite { get; set; }
    }
}