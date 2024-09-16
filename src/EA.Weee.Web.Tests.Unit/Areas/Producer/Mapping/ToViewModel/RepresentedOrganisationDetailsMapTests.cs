namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FluentAssertions;
    using Xunit;

    public class RepresentedOrganisationDetailsMapTests
    {
        private readonly IFixture fixture;
        private readonly RepresentedOrganisationDetailsMap map;

        public RepresentedOrganisationDetailsMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            map = new RepresentedOrganisationDetailsMap();
        }

        [Fact]
        public void Map_ShouldMap()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionData>();

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(source.DirectRegistrantId);
            result.BusinessTradingName.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.BusinessTradingName);
            result.CompanyName.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.CompanyName);

            result.Address.Address1.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.Address1);
            result.Address.Address2.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.Address2);
            result.Address.TownOrCity.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.TownOrCity);
            result.Address.CountyOrRegion.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.CountyOrRegion);
            result.Address.Postcode.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.Postcode);
            result.Address.CountryId.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.CountryId);
            result.Address.Email.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.Email);
            result.Address.Telephone.Should().Be(source.CurrentSubmission.AuthorisedRepresentitiveData.Telephone);
        }
    }
}