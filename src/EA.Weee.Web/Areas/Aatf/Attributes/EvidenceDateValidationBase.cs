namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Api.Client;
    using Filters;
    using Helpers;
    using Prsd.Core;
    using Services;
    using Services.Caching;
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

        private ConfigurationService configService;

        public ConfigurationService ConfigService
        {
            get => configService ?? DependencyResolver.Current.GetService<ConfigurationService>();
            set => configService = value;
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

        public string ApprovalDateValidationMessage { get; private set; }

        public string CompareDatePropertyName { get; private set; }

        public EvidenceDateValidationBase(string compareDatePropertyName)
        {
            CompareDatePropertyName = compareDatePropertyName;
        }

        public EvidenceDateValidationBase(string compareDatePropertyName, string approvalDateValidationMessage)
        {
            CompareDatePropertyName = compareDatePropertyName;
            ApprovalDateValidationMessage = approvalDateValidationMessage;
        }

        public ValidationResult ValidateDateAgainstAatfApprovalDate(DateTime date, Guid organisationId, Guid aatfId)
        {
            var aatfs = AsyncHelpers.RunSync(async () => await Cache.FetchAatfDataForOrganisationData(organisationId));

            var aatfById = aatfs.FirstOrDefault(a => a.Id == aatfId);

            if (aatfById == null)
            {
                return new ValidationResult("Aatf is invalid to save evidence notes.");
            }

            var groupedAatfs = aatfs.Any(a => a.AatfId == 
                aatfById.AatfId && 
                a.ComplianceYear == date.Year &&
                a.ApprovalDate.GetValueOrDefault() <= date);

            if (!groupedAatfs)
            {
                return new ValidationResult(ApprovalDateValidationMessage);
            }

            return ValidationResult.Success;
        }

        protected ValidationResult ValidateStartDate(DateTime startDate, DateTime? endDate, DateTime currentDate)
        {
            if (startDate > new DateTime(currentDate.Year, SystemTime.UtcNow.Month, SystemTime.UtcNow.Day))
            {
                return new ValidationResult("The start date cannot be in the future. Select today's date or earlier.");
            }

            if (endDate.HasValue && !endDate.Equals(DateTime.MinValue))
            {
                if (startDate > endDate.Value.Date)
                {
                    return new ValidationResult("Ensure the start date is before the end date");
                }
            }

            return ValidationResult.Success;
        }

        protected ValidationResult ValidateEndDate(DateTime? endDate, DateTime startDate)
        {
            if (endDate.HasValue && !endDate.Equals(DateTime.MinValue))
            {
                if (startDate < endDate)
                {
                    return new ValidationResult("Ensure the end date is after the start date");
                }
            }

            return ValidationResult.Success;
        }

        protected ValidationResult ValidateDatesAgainstEvidenceNoteSiteSelectionDateFrom(DateTime startDate, DateTime? endDate)
        {
            var configDate = ConfigService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom;

            if (startDate < configDate.Date)
            {
                return new ValidationResult("The start date cannot be before 2023. Evidence notes for compliance years prior to 2023 are not stored in this service.");
            }
            if (endDate.HasValue)
            {
                if (endDate.Value < configDate.Date)
                {
                    return new ValidationResult("The end date cannot be before 2023. Evidence notes for compliance years prior to 2023 are not stored in this service.");
                }
            }

            return ValidationResult.Success;
        }
    }
}