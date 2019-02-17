namespace Archetypical.Software.Vitruvian.Common.Models.Commands
{
    public class UpdateCommand : BaseCommand
    {
        public override Command Command { get; } = Command.Update;
    }
}