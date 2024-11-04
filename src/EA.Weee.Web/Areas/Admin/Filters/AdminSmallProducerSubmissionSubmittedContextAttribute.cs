namespace EA.Weee.Web.Areas.Admin.Filters
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Services.SubmissionService;
    using System;
    using System.Web.Mvc;

    public class AdminSmallProducerSubmissionSubmittedContextAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController;

            var data = controller.SmallProducerSubmissionData;

            if (!data.AnySubmissions)
            {
                context.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "action", nameof(Areas.Admin.Controllers.ProducerSubmissionController.OrganisationHasNoSubmissions) },
                    { "controller", typeof(Areas.Admin.Controllers.ProducerSubmissionController).GetControllerName() },
                    { "organisationId", data.OrganisationData.Id }
                });
            }

            base.OnActionExecuting(context);
        }
    }
}