namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    public class UnknownCommand : BaseCommand
    {
        public override Command Command { get; } = Command.Unknown;
    }
}