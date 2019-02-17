namespace Archetypical.Software.Vitruvian.Common.Models.Responses
{
    public class AddResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.Add;

        public Microsite Microsite { get; set; }
    }
}