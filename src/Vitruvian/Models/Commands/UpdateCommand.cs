namespace Archetypical.Software.Vitruvian.Models.Commands
{
    public class UpdateCommand : BaseCommand
    {
        protected internal override Command Command { get; } = Command.Update;
    }
}