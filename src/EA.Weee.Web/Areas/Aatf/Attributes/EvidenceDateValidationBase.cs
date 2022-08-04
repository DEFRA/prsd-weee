namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Api.Client;
    using Filters;
    using Services;
    using Weee.Requests.Shared;

    public class EvidenceDateValidationBase : ValidationAttribute
    {
        private Func<IWeeeClient> client;
        public Func<IWeeeClient> Client
        {
            get => client ?? DependencyResolver.Current.GetService<Func<IWeeeClient>>();
            set => client = value;
        }

        private IHttpContextService httpContextService;

        public IHttpContextService HttpContextService
        {
            get => httpContextService ?? DependencyResolver.Current.GetService<IHttpContextService>();
            set => httpContextService = value;
        }

        public DateTime GetCurrentDateTime()
        {
            return AsyncHelpers.RunSync(async () =>
            {
                using (var c = Client())
                {
                    return await c.SendAsync(HttpContextService.GetAccessToken(), new GetApiDate());
                }
            });
        }
    }
}