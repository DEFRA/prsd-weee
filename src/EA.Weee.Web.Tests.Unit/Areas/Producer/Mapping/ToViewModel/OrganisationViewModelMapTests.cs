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
    using Xunit;

    public class OrganisationViewModelMapTests
    {
        private readonly IFixture fixture;
        private readonly OrganisationViewModelMap map;
        private readonly IMapper mapper;

        public OrganisationViewModelMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();

            map = new OrganisationViewModelMap(mapper);
        }

        [Fact]
        public void Map_ShouldMapValues()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            var expectedAddress = fixture.Create<ExternalAddressData>();
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            Core.Organisations.Base.OrganisationViewModel result = map.Map(source);

            // Assert
            result.BusinessTradingName.Should().Be(source.OrganisationData.TradingName);
            result.CompanyName.Should().Be(source.OrganisationData.Name);
            result.CompaniesRegistrationNumber.Should().Be(source.OrganisationData.CompanyRegistrationNumber);
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
            result.OrganisationType.Should().Be(expectedType);
        }

        [Fact]
        public void Map_ShouldMapAddress()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            Core.Organisations.Base.OrganisationViewModel result = map.Map(source);

            // Assert
            A.CallTo(() => mapper
                            .Map<AddressData, ExternalAddressData>(source.CurrentSubmission.BusinessAddressData))
                            .MustHaveHappenedOnceExactly();
        }
    }
}