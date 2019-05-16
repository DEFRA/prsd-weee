namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public abstract class ValidateReturnBaseActionFilterAttribute : ActionFilterAttribute
    {
        public abstract Task OnAuthorizationAsync(ActionExecutingContext filterContext);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context));
        }
    }
}