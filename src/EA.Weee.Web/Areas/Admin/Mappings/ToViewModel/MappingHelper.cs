﻿namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using System;

    public static class MappingHelper
    {
        public static T MapFacility<T>(T viewModel, AatfData source)
            where T : FacilityViewModelBase
        {
            viewModel.Id = source.Id;
            viewModel.Name = source.Name;
            viewModel.ApprovalNumber = source.ApprovalNumber;
            viewModel.CompetentAuthorityId = source.CompetentAuthority.Abbreviation;
            viewModel.StatusValue = source.AatfStatus.Value;
            viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
            viewModel.SiteAddressData = source.SiteAddress;
            viewModel.SizeValue = source.Size.Value;
            viewModel.SizeList = Enumeration.GetAll<AatfSize>();
            viewModel.ComplianceYear = source.ComplianceYear;
            viewModel.FacilityType = source.FacilityType;
            if (source.PanAreaData != null)
            {
                viewModel.PanAreaId = source.PanAreaData.Id;
            }
            if (source.LocalAreaData != null)
            {
                viewModel.LocalAreaId = source.LocalAreaData.Id;
            }
            if (source.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.ApprovalDate.GetValueOrDefault();
            }

            return viewModel;
        }

        public static T MapCopyFacility<T>(T viewModel, AatfData source)
           where T : CopyFacilityViewModelBase
        {
            viewModel.Id = source.Id;
            viewModel.Name = source.Name;
            viewModel.ApprovalNumber = source.ApprovalNumber;
            viewModel.CompetentAuthorityId = source.CompetentAuthority.Abbreviation;
            viewModel.StatusValue = source.AatfStatus.Value;
            viewModel.StatusList = Enumeration.GetAll<AatfStatus>();
            viewModel.SiteAddressData = source.SiteAddress;
            viewModel.SizeValue = source.Size.Value;
            viewModel.SizeList = Enumeration.GetAll<AatfSize>();
            viewModel.FacilityType = source.FacilityType;
            if (source.PanAreaData != null)
            {
                viewModel.PanAreaId = source.PanAreaData.Id;
            }
            if (source.LocalAreaData != null)
            {
                viewModel.LocalAreaId = source.LocalAreaData.Id;
            }
            if (source.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.ApprovalDate.GetValueOrDefault();
            }
            viewModel.AatfId = source.AatfId;
            viewModel.ContactData = source.Contact;
            return viewModel;
        }
    }
}