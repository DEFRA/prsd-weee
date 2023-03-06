namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    using System;
    using Xunit;

    public class MappingHelperTests
    {
        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();
            var viewmodel = CreateCopyFacilityViewModel(new CopyAatfViewModel(), aatfData);

            var result = MappingHelper.MapCopyFacility(viewmodel, aatfData);

            AssertResults(aatfData, result);
            Assert.Equal(aatfData.LocalAreaData.Id, result.LocalAreaId);
            Assert.Equal(aatfData.PanAreaData.Id, result.PanAreaId);
        }

        [Fact]
        public void Map_GivenLocalAreaPanArea_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();
            aatfData.PanAreaData = null;
            aatfData.LocalAreaData = null;
            var viewmodel = CreateCopyFacilityViewModel(new CopyAatfViewModel(), aatfData);
            var result = MappingHelper.MapCopyFacility(viewmodel, aatfData);

            AssertResults(aatfData, result);

            Assert.Null(result.LocalAreaId);
            Assert.Null(result.PanAreaId);
        }

        private static void AssertResults(AatfData aatfData, CopyFacilityViewModelBase result)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.AatfStatus.Value, result.StatusValue);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddressData);
            Assert.Equal(aatfData.Size.Value, result.SizeValue);
            Assert.Equal(aatfData.Contact, result.ContactData);
            Assert.Equal(aatfData.Organisation.Id, result.OrganisationId);
            Assert.Equal(aatfData.FacilityType, result.FacilityType);
        }

        private T CreateCopyFacilityViewModel<T>(T viewModel, AatfData aatfData)
            where T : CopyFacilityViewModelBase
        {
            viewModel.ContactData = CreateAatfContactData();
            viewModel.SiteAddressData = CreateAatfAddressData();
            viewModel.SizeValue = aatfData.Size.Value;
            viewModel.StatusValue = aatfData.AatfStatus.Value;
            viewModel.OrganisationId = aatfData.Organisation.Id;
            viewModel.Id = aatfData.Id;
            viewModel.AatfId = aatfData.AatfId;
            return viewModel;
        }

        private UKCompetentAuthorityData CreateUkCompetentAuthorityData()
        {
            return new UKCompetentAuthorityData()
            {
                Abbreviation = "EA",
                CountryId = Guid.NewGuid(),
                Name = "Environmental Agency"
            };
        }

        private AatfAddressData CreateAatfAddressData()
        {
            return new AatfAddressData("ABC", "Here", "There", "Bath", "BANES", "BA2 2PL", Guid.NewGuid(), "England");
        }

        private AatfContactAddressData CreateContactAddressData()
        {
            return new AatfContactAddressData("ABC", "Here", "There", "Bath", "BANES", "BA2 2PL", Guid.NewGuid(), "England");
        }

        private OrganisationData CreateOrganisationData()
        {
            return new OrganisationData()
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                TradingName = "Trading Name",
                CompanyRegistrationNumber = "123456",
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Address1",
                    Address2 = "Address2",
                    CountryName = "France",
                    CountyOrRegion = "County",
                    TownOrCity = "Town",
                    Postcode = "GU22 7UY",
                    Telephone = "987654",
                    Email = "test@test.com"
                }
            };
        }

        private AatfContactData CreateAatfContactData()
        {
            return new AatfContactData(Guid.NewGuid(), "FirstName", "LastName", "Position", CreateContactAddressData(), "Telephone", "Email");
        }

        private AatfData CreateAatfData()
        {
            var competentAuthority = CreateUkCompetentAuthorityData();

            return new AatfData(Guid.NewGuid(), "AatfName", "12345", (Int16)2019, competentAuthority, AatfStatus.Approved, CreateAatfAddressData(), AatfSize.Large, DateTime.Now)
            {
                Contact = CreateAatfContactData(),
                Organisation = CreateOrganisationData(),
                FacilityType = FacilityType.Aatf,
                PanAreaData = new PanAreaData() { Name = "PAN Area", CompetentAuthorityId = competentAuthority.Id },
                LocalAreaData = new LocalAreaData() { Name = "EA Area", CompetentAuthorityId = competentAuthority.Id }
            };
        }
    }
}
