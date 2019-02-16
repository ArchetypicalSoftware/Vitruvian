using Archetypical.Software.Vitruvian.Common;
using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian.Models.Responses
{
    public class ListResponse : BaseResponse
    {
        protected override Command Command { get; } = Command.List;
        public List<Microsite> Microsites { get; set; }
    }
}