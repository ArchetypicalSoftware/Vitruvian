using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public class MicrositeUrlHelperFactory : IUrlHelperFactory
    {
        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            var originalUrlHelperFactory = new UrlHelperFactory();
            var originalUrlHelper = originalUrlHelperFactory.GetUrlHelper(context);
            return new MicrositeUrlHelper(context, originalUrlHelper);
        }
    }
}