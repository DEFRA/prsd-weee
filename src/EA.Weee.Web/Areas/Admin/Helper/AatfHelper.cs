namespace EA.Weee.Web.Areas.Admin.Helper
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AatfHelper
    {
        public static AatfData CreateFacilityData(FacilityViewModelBase viewModel)
        {
            var data = new AatfData(
                Guid.NewGuid(),
                viewModel.Name,
                viewModel.ApprovalNumber,
                viewModel.ComplianceYear,
                viewModel.CompetentAuthoritiesList.FirstOrDefault(p => p.Abbreviation == viewModel.CompetentAuthorityId),
                Enumeration.FromValue<AatfStatus>(viewModel.StatusValue),
                viewModel.SiteAddressData,
                Enumeration.FromValue<AatfSize>(viewModel.SizeValue),
                viewModel.ApprovalDate.GetValueOrDefault(),
                viewModel.PanAreaList.FirstOrDefault(p => p.Id == viewModel.PanAreaId),
                viewModel.LocalAreaList.FirstOrDefault(p => p.Id == viewModel.LocalAreaId))
            { FacilityType = viewModel.FacilityType };

            return data;
        }

        public static List<int> FetchCurrentComplianceYears(DateTime systemDateTime, bool forLinks = false)
        {
            var currentYear = systemDateTime.Year;

            var list = Enumerable.Range(currentYear, 2).ToList();

            if (forLinks)
            {
                list.Add(systemDateTime.AddYears(-1).Year);
            }
            else
            {
                //Until end of Jan show previous year
                DateTime endOfMonth = new DateTime(currentYear, 1,
                                        DateTime.DaysInMonth(currentYear, 1));

                if (systemDateTime <= endOfMonth)
                {
                    list.Add(systemDateTime.AddYears(-1).Year);
                }
            }

            return list.OrderByDescending(x => x).ToList();
        }
    }
}