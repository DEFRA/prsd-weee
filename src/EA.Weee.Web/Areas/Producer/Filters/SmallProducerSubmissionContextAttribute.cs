namespace EA.Weee.Web.Areas.Producer.Filters
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Web.Mvc;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Controllers;

    public class SmallProducerSubmissionContextAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.RouteData.Values.TryGetValue("directRegistrantId", out var directRegistrantIdActionParameter))
            {
                throw new ArgumentException("No direct registrant ID was specified.");
            }

            if (!(Guid.TryParse(directRegistrantIdActionParameter.ToString(), out var guidDirectRegistrantId)))
            {
                throw new ArgumentException("The specified direct registrant ID is not valid.");
            }

            SmallProducerSubmissionData data = null;

            data = AsyncHelpers.RunSync(async () =>
            {
                using (var client = Client())
                {
                    data = await client.SendAsync(context.HttpContext.User.GetAccessToken(), new GetSmallProducerSubmission(guidDirectRegistrantId));

                    if (data == null)
                    {
                        await client.SendAsync(context.HttpContext.User.GetAccessToken(), new AddSmallProducerSubmission(guidDirectRegistrantId));

                        data = await client.SendAsync(context.HttpContext.User.GetAccessToken(), new GetSmallProducerSubmission(guidDirectRegistrantId));
                    }

                    return data;
                }
            });

            if (data == null)
            {
                throw new InvalidOperationException("Producer submission data could not be found");
            }

            switch (context.Controller)
            {
                case ProducerController producerController:
                    producerController.SmallProducerSubmissionData = data;
                    break;
                case ProducerSubmissionController producerSubmissionController:
                    producerSubmissionController.SmallProducerSubmissionData = data;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported controller type");
            }

            base.OnActionExecuting(context);
        }
    }
}