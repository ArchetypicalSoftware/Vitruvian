namespace Archetypical.Software.Vitruvian.Common.Models.Responses
{
    public class UnknownResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.Unknown;
    }
}