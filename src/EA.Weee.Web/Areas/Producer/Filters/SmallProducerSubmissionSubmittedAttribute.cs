namespace EA.Weee.Web.Areas.Producer.Filters
{
    using EA.Weee.Api.Client;
    using System;
    using System.Web.Mvc;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Controllers;

    public class SmallProducerSubmissionSubmittedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var producerController = context.Controller as ProducerController;

            var producerSubmissionController = context.Controller as ProducerSubmissionController;

            if (producerController == null && producerSubmissionController == null)
            {
                throw new InvalidOperationException("Unsupported controller type");
            }

            var submission = producerController == null 
                ? producerSubmissionController.SmallProducerSubmissionData 
                : producerController.SmallProducerSubmissionData;

            if (submission.CurrentSubmission.HasPaid && submission.CurrentSubmission.Status == SubmissionStatus.Submitted)
            {
                context.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "action", "AlreadySubmittedAndPaid" },
                    { "controller", "Producer" }
                });
            }

            base.OnActionExecuting(context);
        }
    }
}