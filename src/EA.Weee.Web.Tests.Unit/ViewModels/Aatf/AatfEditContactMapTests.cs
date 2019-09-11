namespace EA.Weee.Web.Tests.Unit.ViewModels.Aatf
{
    using System;

    using AutoFixture;

    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared.Aatf.Mapping;

    using FluentAssertions;

    using Xunit;

    public class AatfEditContactMapTests
    {
        private readonly AatfEditContactMap mapper;
        private readonly Fixture fixture;

        public AatfEditContactMapTests()
        {
            this.fixture = new Fixture();

            this.mapper = new AatfEditContactMap();
        }

        [Fact]
        public void Map_GivenNullAatfData_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => this.mapper.Map(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_ViewModelPropertiesShouldBeSet()
        {
            var aatfEditContactTransfer = this.fixture.Create<AatfEditContactTransfer>();

            var result = this.mapper.Map(aatfEditContactTransfer);

            result.ContactData.Should().Be(aatfEditContactTransfer.AatfData.Contact);
            result.AatfData.Should().Be(aatfEditContactTransfer.AatfData);
            result.Id.Should().Be(aatfEditContactTransfer.AatfData.Id);
            result.OrganisationId.Should().Be(aatfEditContactTransfer.AatfData.Organisation.Id);
            result.ContactData.AddressData.Countries.Should().BeSameAs(aatfEditContactTransfer.Countries);
        }
    }
}
