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
    using System.Security.AccessControl;
    using Xunit;

    public class SubmissionsOrganisationViewModelMapTests
    {
        private readonly IFixture fixture;
        private readonly SubmissionsOrganisationViewModelMap map;
        private readonly IMapper mapper;

        public SubmissionsOrganisationViewModelMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();

            map = new SubmissionsOrganisationViewModelMap(mapper);
        }

        [Fact]
        public void Map_ShouldMapValues()
        {
            // Arrange
            var source = fixture.Build<SubmissionsYearDetails>()
             .With(x => x.SmallProducerSubmissionData, fixture.Create<SmallProducerSubmissionData>())
             .Create();

            var expectedAddress = fixture.Create<ExternalAddressData>();
            A.CallTo(() => mapper.Map<AddressData, ExternalAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            Core.Organisations.Base.OrganisationViewModel result = map.Map(source);

            // Assert
            result.BusinessTradingName.Should().Be(source.SmallProducerSubmissionData.OrganisationData.TradingName);
            result.CompanyName.Should().Be(source.SmallProducerSubmissionData.OrganisationData.Name);
            result.CompaniesRegistrationNumber.Should().Be(source.SmallProducerSubmissionData.OrganisationData.CompanyRegistrationNumber);
        }

        [Theory]
        [InlineData(OrganisationType.Partnership, ExternalOrganisationType.Partnership)]
        [InlineData(OrganisationType.RegisteredCompany, ExternalOrganisationType.RegisteredCompany)]
        [InlineData(OrganisationType.SoleTraderOrIndividual, ExternalOrganisationType.SoleTrader)]
        public void Map_ShouldMapOrganisationTypeCorrectly(OrganisationType sourceType, ExternalOrganisationType expectedType)
        {
            // Arrange
            var source = fixture.Build<SubmissionsYearDetails>()
                .With(x => x.SmallProducerSubmissionData, fixture.Create<SmallProducerSubmissionData>())
                .With(x => x.SmallProducerSubmissionData.OrganisationData, fixture.Build<OrganisationData>()
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
            var source = fixture.Build<SubmissionsYearDetails>()
            .With(x => x.SmallProducerSubmissionData, fixture.Create<SmallProducerSubmissionData>())
            .Create();

            // Act
            Core.Organisations.Base.OrganisationViewModel result = map.Map(source);

            // Assert
            A.CallTo(() => mapper
                            .Map<AddressData, ExternalAddressData>(source.SmallProducerSubmissionData.OrganisationData.BusinessAddress))
                            .MustHaveHappenedOnceExactly();
        }
    }
}