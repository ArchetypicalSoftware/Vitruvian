using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian.Common.Models.Responses
{
    public class ListResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.List;
        public List<Microsite> Microsites { get; set; }
    }
}