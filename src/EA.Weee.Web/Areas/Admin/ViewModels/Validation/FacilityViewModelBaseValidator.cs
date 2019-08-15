namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using EA.Weee.Web.Infrastructure;
    using FluentValidation;
    using FluentValidation.Results;

    public class FacilityViewModelBaseValidator : AbstractValidator<FacilityViewModelBase>
    {
        private readonly string token;
        private readonly Func<IWeeeClient> apiClient;
        private readonly FacilityViewModelBase model;

        public FacilityViewModelBaseValidator(string token, Func<IWeeeClient> apiClient, FacilityViewModelBase model)
        {
            this.token = token;
            this.apiClient = apiClient;
            this.model = model;

            using (var client = this.apiClient())
            {
                RuleFor(x => x.ApprovalNumber).MustAsync(async (approval, cancellation) => 
                {
                    bool exists = await CheckApprovalNumberIsUnique(approval);
                    return !exists;
                }).WithMessage("Approval number must be unique");
            }
        }

        public async Task<bool> CheckApprovalNumberIsUnique(string approval)
        {
            using (var client = this.apiClient())
            {
                return await client.SendAsync(token, new CheckApprovalNumberIsUnique(approval));
            }
        }
    }
}