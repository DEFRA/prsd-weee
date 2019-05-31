namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using AutoFixture;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using Xunit;

    public class AatfDataToAatfDetailsViewModelMapTests_2
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly AatfDataToAatfDetailsViewModelMap map;
        private readonly Fixture fixture;

        public AatfDataToAatfDetailsViewModelMapTests_2()
        {
            addressUtilities = A.Fake<IAddressUtilities>();
            fixture = new Fixture();

            map = new AatfDataToAatfDetailsViewModelMap(addressUtilities);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => map.Map(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSourceWithNullAaatfData_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => map.Map(new AatfDataToAatfDetailsViewModelMapTransfer(null)));

            exception.Should().BeOfType<ArgumentNullException>();
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
    }
}
