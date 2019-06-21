namespace EA.Weee.Web.Areas.Admin.Requests
{
    using System.Linq;
    using Core.Shared;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public class EditFacilityDetailsRequestCreator : IEditFacilityDetailsRequestCreator
    {
        public EditAatfDetails ViewModelToRequest(FacilityViewModelBase viewModel)
        {
            var data = new AatfData(
                viewModel.Id,
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Abbreviation == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault());

            if (viewModel.CompetentAuthorityId != UKCompetentAuthorityAbbreviationType.EA)
            {
                data.PanAreaData = null;
                data.LocalAreaData = null;
            }
            else
            {
                data.PanAreaData = viewModel.PanAreaList.FirstOrDefault(p => p.Id == viewModel.PanAreaId);
                data.LocalAreaData = viewModel.LocalAreaList.First(p => p.Id == viewModel.LocalAreaId);
            }

            data.FacilityType = viewModel.FacilityType;

            return new EditAatfDetails()
            {
                Data = data
            };
        }
    }
}