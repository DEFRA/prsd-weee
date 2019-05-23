namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.AatfReturn.Attributes;

    public class ValidateReturnTestActionFilterAttribute : ValidateReturnBaseActionFilterAttribute
    {
        public override Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid returnId)
        {
            return Task.CompletedTask;
        }
    }
}
