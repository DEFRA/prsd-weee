namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AatfDataToAatfDetailsViewModelMapTests
    {
        private readonly AatfDataToAatfDetailsViewModel map;

        public AatfDataToAatfDetailsViewModelMapTests()
        {
            map = new AatfDataToAatfDetailsViewModel(new AddressUtilities());
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

            var transfer = new AatfDataToAatfDetailsViewModelTransfer(aatfData, aatfContactData);

            AatfDetailsViewModel result = map.Map(transfer);
            AssertResults(aatfData, aatfContactData, result);
            Assert.NotNull(result.ApprovalDate);
        }

        [Fact]
        public void Map_GivenValidSource_WithNoApprovalDate_PropertiesShouldBeMapped_ApprovalDateShouldBeDefaultDatetime()
        {
            AatfData aatfData = CreateAatfData();
            AatfContactData aatfContactData = CreateAatfContactData();
            aatfData.ApprovalDate = default(DateTime);

            var transfer = new AatfDataToAatfDetailsViewModelTransfer(aatfData, aatfContactData);

            AatfDetailsViewModel result = map.Map(transfer);

            AssertResults(aatfData, aatfContactData, result);
            Assert.Null(result.ApprovalDate);
        }

        private static void AssertResults(AatfData aatfData, AatfContactData aatfContactData, AatfDetailsViewModel result)
        {
            Assert.Equal(aatfData.Id, result.Id);
            Assert.Equal(aatfData.Name, result.Name);
            Assert.Equal(aatfData.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(aatfData.CompetentAuthority, result.CompetentAuthority);
            Assert.Equal(aatfData.AatfStatus, result.AatfStatus);
            Assert.Equal(aatfData.SiteAddress, result.SiteAddress);
            Assert.Equal(aatfData.Size, result.Size);
            Assert.Equal(aatfContactData, result.ContactData);
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

        private AatfContactData CreateAatfContactData()
        {
            return new AatfContactData(Guid.NewGuid(), "FirstName", "LastName", "Position", CreateContactAddressData(), "Telephone", "Email");
        }

        private AatfData CreateAatfData()
        {
            return new AatfData(Guid.NewGuid(), "AatfName", "12345", CreateUkCompetentAuthorityData(), AatfStatus.Approved, CreateAatfAddressData(), AatfSize.Large, DateTime.Now);
        }
    }
}
