namespace Archetypical.Software.Vitruvian.Models.Commands
{
    public class ListCommand : BaseCommand
    {
        protected internal override Command Command { get; } = Command.List;
    }
}