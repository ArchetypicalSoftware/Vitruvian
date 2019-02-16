namespace Archetypical.Software.Vitruvian.Models.Commands
{
    public class DeleteCommand : BaseCommand
    {
        protected internal override Command Command { get; } = Command.Delete;
    }
}