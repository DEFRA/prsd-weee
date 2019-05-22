namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class AatfDataToAatfDetailsViewModelMapTests
    {
        private readonly AatfDataToAatfDetailsViewModelMap map;

        public AatfDataToAatfDetailsViewModelMapTests()
        {
            map = new AatfDataToAatfDetailsViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_WithApprovalDate_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();
            AatfContactData aatfContactData = CreateAatfContactData();
            OrganisationData organisationData = CreateOrganisationData();

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData, aatfContactData, organisationData);

            AatfDetailsViewModel result = map.Map(transfer);
            AssertResults(aatfData, aatfContactData, organisationData, result);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithAatfs_AEListShouldOnlyContainAEsAndAatfListShouldOnlyContainAatfs()
        {
            AatfData aatfData = CreateAatfData();
            AatfContactData aatfContactData = CreateAatfContactData();
            OrganisationData organisationData = CreateOrganisationData();
            List<AatfDataList> associatedAatfs = new List<AatfDataList>();
            associatedAatfs.Add(new AatfDataList(Guid.NewGuid(), "TEST", A.Fake<UKCompetentAuthorityData>(), "123456789", A.Fake<AatfStatus>(), A.Fake<OperatorData>(), FacilityType.Aatf));
            associatedAatfs.Add(new AatfDataList(Guid.NewGuid(), "TEST", A.Fake<UKCompetentAuthorityData>(), "123456789", A.Fake<AatfStatus>(), A.Fake<OperatorData>(), FacilityType.Ae));

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData, aatfContactData, organisationData)
            {
                AssociatedAatfs = associatedAatfs
            };

            AatfDetailsViewModel result = map.Map(transfer);

            foreach (var ae in result.AssociatedAEs)
            {
                ae.FacilityType.Should().Be(FacilityType.Ae);
            }

            foreach (var aatf in result.AssociatedAatfs)
            {
                aatf.FacilityType.Should().Be(FacilityType.Aatf);
            }
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDatetime()
        {
            AatfData aatfData = CreateAatfData();
            AatfContactData aatfContactData = CreateAatfContactData();
            OrganisationData organisationData = CreateOrganisationData();
            aatfData.ApprovalDate = default(DateTime);

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData, aatfContactData, organisationData);

            AatfDetailsViewModel result = map.Map(transfer);

            AssertResults(aatfData, aatfContactData, organisationData, result);
            Assert.Null(result.ApprovalDate);
        }

        private static void AssertResults(AatfData aatfData, AatfContactData aatfContactData, OrganisationData organisationData, AatfDetailsViewModel result)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.CompetentAuthority, result.CompetentAuthority);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddress);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Equal(aatfContactData, result.ContactData);
            Assert.Equal(organisationData.Name, result.Organisation.Name);
            Assert.Equal(organisationData.TradingName, result.Organisation.TradingName);
            Assert.Equal(organisationData.CompanyRegistrationNumber, result.Organisation.CompanyRegistrationNumber);
            Assert.Equal(organisationData.BusinessAddress.Address1, result.Organisation.BusinessAddress.Address1);
            Assert.Equal(organisationData.BusinessAddress.Address2, result.Organisation.BusinessAddress.Address2);
            Assert.Equal(organisationData.BusinessAddress.CountyOrRegion, result.Organisation.BusinessAddress.CountyOrRegion);
            Assert.Equal(organisationData.BusinessAddress.CountryName, result.Organisation.BusinessAddress.CountryName);
            Assert.Equal(organisationData.BusinessAddress.TownOrCity, result.Organisation.BusinessAddress.TownOrCity);
            Assert.Equal(organisationData.BusinessAddress.Postcode, result.Organisation.BusinessAddress.Postcode);
            Assert.Equal(organisationData.BusinessAddress.Telephone, result.Organisation.BusinessAddress.Telephone);
            Assert.Equal(organisationData.BusinessAddress.Email, result.Organisation.BusinessAddress.Email);
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

        private OperatorData CreateOperatorData()
        {
            return new OperatorData(Guid.NewGuid(), "Operator", Guid.NewGuid());
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
            return new AatfData(Guid.NewGuid(), "AatfName", "12345", CreateOperatorData(), CreateUkCompetentAuthorityData(), AatfStatus.Approved, CreateAatfAddressData(), AatfSize.Large, DateTime.Now);
        }
    }
}
