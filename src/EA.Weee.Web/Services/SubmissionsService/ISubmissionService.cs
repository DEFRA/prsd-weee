namespace EA.Weee.Web.Services.SubmissionsService
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System;
    using System.Threading.Tasks;

    public interface ISubmissionService
    {
        Task<OrganisationDetailsTabsViewModel> ContactDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> OrganisationDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> RepresentedOrganisationDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> ServiceOfNoticeDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> Submissions(int? year = null);
        Task<OrganisationDetailsTabsViewModel> TotalEEEDetails(int? year = null);
        SubmissionsService.SubmissionService WithSubmissionData(SmallProducerSubmissionData data, bool isInternal = false, int? year = null);
    }
}