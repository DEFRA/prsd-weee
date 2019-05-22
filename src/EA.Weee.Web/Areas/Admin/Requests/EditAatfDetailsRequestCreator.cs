namespace EA.Weee.Web.Areas.Admin.Requests
{
    using System.Linq;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public class EditAatfDetailsRequestCreator : IEditAatfDetailsRequestCreator
    {
        public EditAatfDetails ViewModelToRequest(AatfEditDetailsViewModel viewModel)
        {
            return new EditAatfDetails()
            {
                Data = new AatfData(
                    viewModel.Id,
                    viewModel.Name,
                    viewModel.ApprovalNumber,
                    viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Id == viewModel.CompetentAuthorityId),
                    Enumeration.FromValue<AatfStatus>(viewModel.AatfStatus),
                    viewModel.SiteAddress,
                    Enumeration.FromValue<AatfSize>(viewModel.Size),
                    viewModel.ApprovalDate.GetValueOrDefault())
            };
        }
    }
}