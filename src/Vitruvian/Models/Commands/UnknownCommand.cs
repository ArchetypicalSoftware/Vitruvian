namespace Archetypical.Software.Vitruvian.Models.Commands
{
    public class UnknownCommand : BaseCommand
    {
        protected internal override Command Command { get; } = Command.Unknown;
    }
}