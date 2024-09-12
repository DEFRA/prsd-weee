namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class EditOrganisationDetailsMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly EditOrganisationDetailsMap map;

        public EditOrganisationDetailsMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();
            map = new EditOrganisationDetailsMap(mapper);
        }

        [Fact]
        public void Map_ShouldMapHighLevelSourceFields()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(source.DirectRegistrantId);
            result.OrganisationId.Should().Be(source.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(source.HasAuthorisedRepresentitive);
        }

        [Theory]
        [InlineData(OrganisationType.Partnership, ExternalOrganisationType.Partnership)]
        [InlineData(OrganisationType.RegisteredCompany, ExternalOrganisationType.RegisteredCompany)]
        [InlineData(OrganisationType.SoleTraderOrIndividual, ExternalOrganisationType.SoleTrader)]
        public void Map_ShouldMapOrganisationTypeCorrectly(OrganisationType sourceType, ExternalOrganisationType expectedType)
        {
            // Arrange
            var source = fixture.Build<SmallProducerSubmissionData>()
                .With(x => x.OrganisationData, fixture.Build<OrganisationData>()
                    .With(o => o.OrganisationType, sourceType)
                    .Create())
                .Create();

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.OrganisationType.Should().Be(expectedType);
        }

        [Fact]
        public void Map_ShouldUseCurrentSubmissionDataWhenAvailable()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.CompanyName.Should().Be(source.CurrentSubmission.CompanyName);
            result.Organisation.BusinessTradingName.Should().Be(source.CurrentSubmission.TradingName);
            result.Organisation.EEEBrandNames.Should().Be(source.CurrentSubmission.EEEBrandNames);
            result.Organisation.CompaniesRegistrationNumber.Should().Be(source.OrganisationData.CompanyRegistrationNumber);
        }

        [Fact]
        public void Map_ShouldMapBusinessAddress()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();
            var expectedAddress = fixture.Create<ExternalAddressData>();
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.Organisation.Address.Should().Be(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(source.CurrentSubmission.BusinessAddressData))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapAdditionalContactDetails()
        {
            // Arrange
            var source = fixture.Build<SmallProducerSubmissionData>()
                .With(x => x.CurrentSubmission, fixture.Build<SmallProducerSubmissionHistoryData>()
                    .With(s => s.AdditionalCompanyDetailsData, new List<AdditionalCompanyDetailsData>
                    {
                        new AdditionalCompanyDetailsData { FirstName = "John", LastName = "Doe" },
                        new AdditionalCompanyDetailsData { FirstName = "Jane", LastName = "Smith" }
                    })
                    .Create())
                .Create();

            // Act
            var result = map.Map(source);

            // Assert
            result.AdditionalContactModels.Should().HaveCount(2);
            result.AdditionalContactModels[0].FirstName.Should().Be("John");
            result.AdditionalContactModels[0].LastName.Should().Be("Doe");
            result.AdditionalContactModels[1].FirstName.Should().Be("Jane");
            result.AdditionalContactModels[1].LastName.Should().Be("Smith");
        }
    }
}