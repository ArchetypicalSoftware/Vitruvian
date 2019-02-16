namespace Archetypical.Software.Vitruvian.Models.Responses
{
    public class UnknownResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.Unknown;
    }
}