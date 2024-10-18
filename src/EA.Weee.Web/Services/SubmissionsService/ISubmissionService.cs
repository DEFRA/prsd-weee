namespace EA.Weee.Web.Services.SubmissionService
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System.Threading.Tasks;

    public interface ISubmissionService
    {
        Task<OrganisationDetailsTabsViewModel> ContactDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> OrganisationDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> RepresentedOrganisationDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> ServiceOfNoticeDetails(int? year = null);
        Task<OrganisationDetailsTabsViewModel> Submissions(int? year = null);
        Task<OrganisationDetailsTabsViewModel> TotalEEEDetails(int? year = null);
        SubmissionService WithSubmissionData(SmallProducerSubmissionData data, bool isInternal = false);
    }
}