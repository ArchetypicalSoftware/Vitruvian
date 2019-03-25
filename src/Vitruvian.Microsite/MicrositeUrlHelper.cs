using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public class MicrositeUrlHelper : IUrlHelper
    {
        private readonly IUrlHelper _originalUrlHelper;

        public ActionContext ActionContext { get; }

        private MicrositeConfiguration Configuration { get; }

        public MicrositeUrlHelper(ActionContext actionContext, IUrlHelper originalUrlHelper)
        {
            ActionContext = actionContext;
            _originalUrlHelper = originalUrlHelper;
            Configuration = actionContext.HttpContext.RequestServices.GetService<MicrositeConfiguration>();
        }

        public string Action(UrlActionContext urlActionContext)
        {
            return Configuration.Slug + _originalUrlHelper.Action(urlActionContext);
        }

        public string Content(string contentPath)
        {
            return _originalUrlHelper.Content(contentPath);
        }

        public bool IsLocalUrl(string url)
        {
            return _originalUrlHelper.IsLocalUrl(url);
        }

        public string Link(string routeName, object values)
        {
            return _originalUrlHelper.Link(routeName, values);
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            return _originalUrlHelper.RouteUrl(routeContext);
        }
    }
}