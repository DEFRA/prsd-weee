namespace EA.Weee.Web.Areas.Admin.Filters
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Web.Mvc;

    public class AdminSmallProducerSubmissionContextAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var registrationNumber = context.HttpContext.Request.QueryString["RegistrationNumber"];
           
            if (registrationNumber == null)
            {
                throw new ArgumentException("No RegistrationNumber was specified.");
            }

            var data = AsyncHelpers.RunSync(async () =>
            {
                using (var client = Client())
                {
                    return await client.SendAsync(context.HttpContext.User.GetAccessToken(), new GetSmallProducerSubmissionByRegistrationNumber(registrationNumber));
                }
            });

            if (data == null)
            {
                throw new InvalidOperationException("Producer submission data could not be found");
            }

            var controller = context.Controller as EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController;

            controller.SmallProducerSubmissionData = data;

            base.OnActionExecuting(context);
        }
    }
}