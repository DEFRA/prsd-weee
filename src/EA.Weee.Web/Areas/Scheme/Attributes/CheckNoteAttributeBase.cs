namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Filters;
    using Infrastructure;
    using Services.Caching;
    using Weee.Requests.Shared;

    public abstract class CheckNoteAttributeBase : ActionFilterAttribute
    {
        public IWeeeCache Cache { get; set; }

        public Func<IWeeeClient> Client { get; set; }

        public abstract Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var pcsId = TryGetPcsId(context);

            AsyncHelpers.RunSync(async () =>
            {
                try
                {
                    await OnAuthorizationAsync(context, pcsId);
                }
                catch (InvalidOperationException)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                    {
                        { "action", "Index" },
                        { "controller", "ManageEvidenceNotes" }
                    });
                }
            });

            base.OnActionExecuting(context);
        }

        protected Guid TryGetPcsId(ActionExecutingContext filterContext)
        {
            if (!filterContext.RouteData.Values.TryGetValue("pcsId", out var idActionParameter))
            {
                throw new ArgumentException("No pcs ID was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var pcsIdActionParameter)))
            {
                throw new ArgumentException("The specified pcs ID is not valid.");
            }

            return pcsIdActionParameter;
        }

        protected Guid TryGetEvidenceNoteId(ActionExecutingContext filterContext)
        {
            if (!filterContext.RouteData.Values.TryGetValue("evidenceNoteId", out var idActionParameter))
            {
                throw new ArgumentException("No evidence note id specified");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var evidenceNoteIdActionParameter)))
            {
                throw new ArgumentException("The specified evidence note id is not valid.");
            }

            return evidenceNoteIdActionParameter;
        }

        protected int TryGetComplianceYear(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.TryGetValue("complianceYear", out var idActionParameter))
            {
                throw new ArgumentException("No compliance year was specified.");
            }

            if (!(int.TryParse(idActionParameter.ToString(), out var complianceYearIdActionParameter)))
            {
                throw new ArgumentException("The specified compliance year is not valid.");
            }

            return complianceYearIdActionParameter;
        }

        public async Task<DateTime> GetCurrentDate(HttpContextBase httpContext)
        {
            using (var c = Client())
            {
                return await c.SendAsync(httpContext.User.GetAccessToken(), new GetApiUtcDate());
            }
        }
    }
}