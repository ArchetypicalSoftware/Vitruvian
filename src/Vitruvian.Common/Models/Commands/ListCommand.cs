namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    public class ListCommand : BaseCommand
    {
        public override Command Command { get; } = Command.List;
    }
}