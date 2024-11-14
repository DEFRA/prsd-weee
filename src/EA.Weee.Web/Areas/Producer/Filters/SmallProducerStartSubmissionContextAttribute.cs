﻿namespace EA.Weee.Web.Areas.Producer.Filters
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Web.Mvc;

    public class SmallProducerStartSubmissionContextAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public IWeeeCache Cache { get; set; }

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

            AsyncHelpers.RunSync(async () =>
            {
                using (var client = Client())
                {
                    data = await client.SendAsync(context.HttpContext.User.GetAccessToken(), new GetSmallProducerSubmission(guidDirectRegistrantId));

                    if (data?.CurrentSubmission == null)
                    {
                        var addSubmissionResult = await client.SendAsync(context.HttpContext.User.GetAccessToken(), new AddSmallProducerSubmission(guidDirectRegistrantId));

                        if (addSubmissionResult.InvalidCache)
                        {
                            await Cache.InvalidateSmallProducerSearch();
                        }
                    }
                }
            });

            base.OnActionExecuting(context);
        }
    }
}