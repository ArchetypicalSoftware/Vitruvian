using Archetypical.Software.Vitruvian.Common;

namespace Archetypical.Software.Vitruvian.Models.Responses
{
    public class AddResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.Add;

        public Microsite Microsite { get; set; }
    }
}