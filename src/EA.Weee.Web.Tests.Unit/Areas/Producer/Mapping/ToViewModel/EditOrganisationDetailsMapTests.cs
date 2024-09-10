namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class EditOrganisationDetailsMapTests
    {
        private readonly IMapper mapper;
        private readonly EditOrganisationDetailsMap map;

        public EditOrganisationDetailsMapTests()
        {
            mapper = A.Fake<IMapper>();
            map = new EditOrganisationDetailsMap(mapper);
        }

        [Fact]
        public void Map_ShouldMapDirectRegistrantId()
        {
            // Arrange
            var source = CreateTestSource();

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(source.DirectRegistrantId);
        }

        [Theory]
        [InlineData(OrganisationType.Partnership, ExternalOrganisationType.Partnership)]
        [InlineData(OrganisationType.RegisteredCompany, ExternalOrganisationType.RegisteredCompany)]
        [InlineData(OrganisationType.SoleTraderOrIndividual, ExternalOrganisationType.SoleTrader)]
        public void Map_ShouldMapOrganisationTypeCorrectly(OrganisationType sourceType, ExternalOrganisationType expectedType)
        {
            // Arrange
            var source = CreateTestSource(organisationType: sourceType);

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.OrganisationType.Should().Be(expectedType);
        }

        [Fact]
        public void Map_ShouldUseCurrentSubmissionDataWhenAvailable()
        {
            // Arrange
            var source = CreateTestSource();

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.CompanyName.Should().Be(source.CurrentSubmission.CompanyName);
            result.Organisation.BusinessTradingName.Should().Be(source.CurrentSubmission.TradingName);
            result.Organisation.EEEBrandNames.Should().BeEquivalentTo(source.CurrentSubmission.EEEBrandNames);
        }

        [Fact]
        public void Map_ShouldMapBusinessAddress()
        {
            // Arrange
            var submissionBusinessAddress = new AddressData();
            var source = CreateTestSource(currentSubmissionAddressData: submissionBusinessAddress);
            var expectedAddress = new ExternalAddressData();
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.Address.Should().BeSameAs(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(source.CurrentSubmission.BusinessAddressData))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldUseOrganisationBusinessAddressWhenCurrentSubmissionAddressIsNull()
        {
            // Arrange
            var source = CreateTestSource(currentSubmissionAddressData: null);
            var expectedAddress = new ExternalAddressData();
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.Address.Should().BeSameAs(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(source.OrganisationData.BusinessAddress))
                .MustHaveHappenedOnceExactly();
        }

        private SmallProducerSubmissionData CreateTestSource(
            OrganisationType organisationType = OrganisationType.RegisteredCompany,
            AddressData currentSubmissionAddressData = null)
        {
            return new SmallProducerSubmissionData
            {
                DirectRegistrantId = Guid.NewGuid(),
                OrganisationData = new OrganisationData
                {
                    OrganisationType = organisationType,
                    BusinessAddress = new AddressData()
                },
                CurrentSubmission = new SmallProducerSubmissionHistoryData()
                {
                    CompanyName = "Test Company",
                    TradingName = "Test Trading Name",
                    EEEBrandNames = "Brand1",
                    BusinessAddressData = currentSubmissionAddressData
                }
            };
        }
    }
}