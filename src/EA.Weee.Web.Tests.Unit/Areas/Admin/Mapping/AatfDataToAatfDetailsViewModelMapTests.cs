namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using AutoFixture;
    using Core.Admin.AatfReports;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Areas.Admin.ViewModels.Shared;
    using Xunit;

    public class AatfDataToAatfDetailsViewModelMapTests
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel> aatfSubmissionHistoryMap;
        private readonly AatfDataToAatfDetailsViewModelMap map;
        private readonly IMapper mapper;
        private readonly Fixture fixture;

        public AatfDataToAatfDetailsViewModelMapTests()
        {
            addressUtilities = A.Fake<IAddressUtilities>();
            aatfSubmissionHistoryMap = A.Fake<IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel>>();
            mapper = A.Fake<IMapper>();

            fixture = new Fixture();

            map = new AatfDataToAatfDetailsViewModelMap(addressUtilities, aatfSubmissionHistoryMap, mapper);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSourceWithNullAaatfData_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => map.Map(new AatfDataToAatfDetailsViewModelMapTransfer(null)));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_WithApprovalDate_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData);

            AatfDetailsViewModel result = map.Map(transfer);
            AssertResults(aatfData, result);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDatetime()
        {
            AatfData aatfData = CreateAatfData();
            aatfData.ApprovalDate = default(DateTime);

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData);

            AatfDetailsViewModel result = map.Map(transfer);

            AssertResults(aatfData, result);
            Assert.Null(result.ApprovalDate);
        }

        private static void AssertResults(AatfData aatfData, AatfDetailsViewModel result)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.CompetentAuthority, result.CompetentAuthority);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddress);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Equal(aatfData.Contact, result.ContactData);
            Assert.Equal(aatfData.Organisation.Name, result.Organisation.Name);
            Assert.Equal(aatfData.Organisation.TradingName, result.Organisation.TradingName);
            Assert.Equal(aatfData.Organisation.CompanyRegistrationNumber, result.Organisation.CompanyRegistrationNumber);
            Assert.Equal(aatfData.Organisation.BusinessAddress.Address1, result.Organisation.BusinessAddress.Address1);
            Assert.Equal(aatfData.Organisation.BusinessAddress.Address2, result.Organisation.BusinessAddress.Address2);
            Assert.Equal(aatfData.Organisation.BusinessAddress.CountyOrRegion, result.Organisation.BusinessAddress.CountyOrRegion);
            Assert.Equal(aatfData.Organisation.BusinessAddress.CountryName, result.Organisation.BusinessAddress.CountryName);
            Assert.Equal(aatfData.Organisation.BusinessAddress.TownOrCity, result.Organisation.BusinessAddress.TownOrCity);
            Assert.Equal(aatfData.Organisation.BusinessAddress.Postcode, result.Organisation.BusinessAddress.Postcode);
            Assert.Equal(aatfData.Organisation.BusinessAddress.Telephone, result.Organisation.BusinessAddress.Telephone);
            Assert.Equal(aatfData.Organisation.BusinessAddress.Email, result.Organisation.BusinessAddress.Email);
            Assert.Equal(aatfData.FacilityType, result.FacilityType);
            Assert.Equal(aatfData.LocalAreaData, result.LocalArea);
            Assert.Equal(aatfData.PanAreaData, result.PanArea);
            Assert.Equal(aatfData.AatfId, result.AatfId);
            Assert.Equal(aatfData.ComplianceYear, result.SelectedComplianceYear);
        }

        [Fact]
        public void Map_GivenAddresses_AddressPropertiesShouldBeMapped()
        {
            var model = fixture.Build<AatfDataToAatfDetailsViewModelMapTransfer>()
                .WithAutoProperties()
                .Create();
            const string siteAddress = "address";
            const string contactAddress = "contactAddress";

            A.CallTo(() => addressUtilities.FormattedAddress(model.AatfData.SiteAddress, false)).Returns(siteAddress);
            A.CallTo(() => addressUtilities.FormattedAddress(model.AatfData.Contact.AddressData, false)).Returns(contactAddress);

            var result = map.Map(model);

            result.ContactAddressLong.Should().Be(contactAddress);
            result.SiteAddressLong.Should().Be(siteAddress);
            result.OrganisationAddress.Should().Be(model.OrganisationString);
        }

        [Fact]
        public void Map_GivenNullSubmissionHistoryOnTransfer_SubmissionHistoryDataShouldBeNull()
        {
            var source = fixture.Build<AatfDataToAatfDetailsViewModelMapTransfer>()
                .Without(s => s.SubmissionHistory)
                .Create();

            var result = map.Map(source);

            result.SubmissionHistoryData.Should().BeNull();
        }

        [Fact]
        public void Map_GivenSubmissionHistoryOnTransfer_SubmissionHistoryDataShouldBeMapped()
        {
            var source = fixture.Build<AatfDataToAatfDetailsViewModelMapTransfer>().Create();

            var result = map.Map(source);

            foreach (var aatfSubmissionHistoryData in source.SubmissionHistory)
            {
                A.CallTo(() => aatfSubmissionHistoryMap.Map(aatfSubmissionHistoryData)).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Map_GivenSubmissionHistoryOnTransfer_MappedSubmissionHistoryDataShouldBeReturned()
        {
            var aatSubmissionHistoryData = new List<AatfSubmissionHistoryData>()
            {
                fixture.Create<AatfSubmissionHistoryData>(),
                fixture.Create<AatfSubmissionHistoryData>()
            };

            var aatSubmissionHistoryViewModel = new List<AatfSubmissionHistoryViewModel>()
            {
                fixture.Create<AatfSubmissionHistoryViewModel>(),
                fixture.Create<AatfSubmissionHistoryViewModel>()
            };

            A.CallTo(() => aatfSubmissionHistoryMap.Map(aatSubmissionHistoryData.ElementAt(0))).Returns(aatSubmissionHistoryViewModel.ElementAt(0));
            A.CallTo(() => aatfSubmissionHistoryMap.Map(aatSubmissionHistoryData.ElementAt(1))).Returns(aatSubmissionHistoryViewModel.ElementAt(1));

            var source = fixture.Build<AatfDataToAatfDetailsViewModelMapTransfer>()
                .With(s => s.SubmissionHistory, aatSubmissionHistoryData).Create();

            var result = map.Map(source);

            result.SubmissionHistoryData.ElementAt(0).Should().BeEquivalentTo(aatSubmissionHistoryViewModel.ElementAt(0));
            result.SubmissionHistoryData.ElementAt(1).Should().BeEquivalentTo(aatSubmissionHistoryViewModel.ElementAt(1));
        }

        [Fact]
        public void Map_GivenValidSource_WithComplianceYears_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();
            var complianceYears = A.Fake<List<short>>();

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData)
            {
                ComplianceYearList = complianceYears,
            };

            AatfDetailsViewModel result = map.Map(transfer);

            result.ComplianceYearList.Should().BeEquivalentTo(complianceYears);
        }

        [Fact]
        public void Map_GivenValidSource_WithCurrentDate_PropertiesShouldBeMapped()
        {
            AatfData aatfData = CreateAatfData();
            var date = SystemTime.UtcNow;

            var transfer = new AatfDataToAatfDetailsViewModelMapTransfer(aatfData)
            {
                CurrentDate = date,
            };

            AatfDetailsViewModel result = map.Map(transfer);

            result.CurrentDate.Should().Be(date);
        }

        [Fact]
        public void Map_GivenSource_AssociatedEntitiesMapperMustBeCalled()
        {
            var transfer = fixture.Create<AatfDataToAatfDetailsViewModelMapTransfer>();

            var result = map.Map(transfer);

            A.CallTo(() => mapper.Map<AssociatedEntitiesViewModel>(A<AssociatedEntitiesViewModelTransfer>.That.Matches(a =>
                a.AatfId == transfer.AatfData.AatfId &&
                a.AssociatedAatfs == transfer.AssociatedAatfs &&
                a.AssociatedSchemes == transfer.AssociatedSchemes))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenSource_MappedAssociatedViewModelIsReturned()
        {
            var transfer = fixture.Create<AatfDataToAatfDetailsViewModelMapTransfer>();
            var associatedViewModel = fixture.Create<AssociatedEntitiesViewModel>();

            A.CallTo(() => mapper.Map<AssociatedEntitiesViewModel>(A<AssociatedEntitiesViewModelTransfer>._)).Returns(associatedViewModel);

            var result = map.Map(transfer);

            result.AssociatedEntities.Should().Be(associatedViewModel);
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
                LocalAreaData = new LocalAreaData() { Name = "EA Area", CompetentAuthorityId = competentAuthority.Id },
                AatfId = Guid.NewGuid()
            };
        }
    }
}
