namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Antlr.Runtime;
    using Api.Client;
    using Filters;
    using Helpers;
    using Prsd.Core;
    using Services;
    using Services.Caching;
    using Weee.Requests.AatfReturn;
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

        private IWeeeCache cache;

        public IWeeeCache Cache
        {
            get => cache ?? DependencyResolver.Current.GetService<IWeeeCache>();
            set => cache = value;
        }

        private IAatfEvidenceHelper aatfEvidenceHelper;

        public IAatfEvidenceHelper AatfEvidenceHelper
        {
            get => aatfEvidenceHelper ?? DependencyResolver.Current.GetService<IAatfEvidenceHelper>();
            set => aatfEvidenceHelper = value;
        }

        public DateTime GetCurrentDateTime()
        {
            return AsyncHelpers.RunSync(() =>
            {
                using (var c = Client())
                {
                    return c.SendAsync(HttpContextService.GetAccessToken(), new GetApiDate());
                }
            });
        }

        public ValidationResult ValidateDateAgainstAatfApprovalDate(DateTime date, Guid organisationId, Guid aatfId)
        {
            var aatfs = AsyncHelpers.RunSync(() =>
            {
                using (var c = Client())
                {
                    return c.SendAsync(HttpContextService.GetAccessToken(), new GetAatfByOrganisation(organisationId));
                }
            });

            var groupedAatfs = aatfEvidenceHelper.GroupedValidAatfs(aatfs);

            var aatf = groupedAatfs.FirstOrDefault(a => a.ComplianceYear == date.Year);

            if (aatf != null)
            {
                if (aatf.ApprovalDate > date)
                {
                    return new ValidationResult("The start date cannot be in the future. Select today's date or earlier.");
                }
            }

            return ValidationResult.Success;
        }

        protected ValidationResult ValidateStartDate(DateTime thisDate, DateTime? otherDate, DateTime currentDate)
        {
            if (thisDate > new DateTime(currentDate.Year, SystemTime.UtcNow.Month, SystemTime.UtcNow.Day))
            {
                return new ValidationResult("The start date cannot be in the future. Select today's date or earlier.");
            }

            if (otherDate.HasValue && !otherDate.Equals(DateTime.MinValue))
            {
                if (thisDate > otherDate.Value.Date)
                {
                    return new ValidationResult("Ensure the start date is before the end date");
                }
            }

            return ValidationResult.Success;
        }

        protected ValidationResult ValidateEndDate(DateTime? otherDate, DateTime thisDate)
        {
            if (otherDate.HasValue && !otherDate.Equals(DateTime.MinValue))
            {
                if (thisDate < otherDate)
                {
                    return new ValidationResult("Ensure the end date is after the start date");
                }
            }

            return ValidationResult.Success;
        }
    }
}