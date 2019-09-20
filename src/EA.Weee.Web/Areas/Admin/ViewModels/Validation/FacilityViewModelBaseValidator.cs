namespace EA.Weee.Web.Areas.Admin.ViewModels.Validation
{
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentValidation;
    using System;
    using System.Threading.Tasks;

    public class FacilityViewModelBaseValidator : AbstractValidator<FacilityViewModelBase>
    {
        private readonly string token;
        private readonly Func<IWeeeClient> apiClient;

        public FacilityViewModelBaseValidator(string token, Func<IWeeeClient> apiClient, int? year)
        {
            this.token = token;
            this.apiClient = apiClient;

            using (var client = this.apiClient())
            {
                RuleFor(x => x.ApprovalNumber).MustAsync(async (approval, cancellation) =>
                {
                    var exists = await CheckApprovalNumberIsUnique(approval, year);
                    return !exists;
                }).WithMessage("Approval number must be unique");
            }
        }

        public async Task<bool> CheckApprovalNumberIsUnique(string approval, int? year)
        {
            using (var client = this.apiClient())
            {
                return await client.SendAsync(token, new CheckApprovalNumberIsUnique(approval, year));
            }
        }
    }
}