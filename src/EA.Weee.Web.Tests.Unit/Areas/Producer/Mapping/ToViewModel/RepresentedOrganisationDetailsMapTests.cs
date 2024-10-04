namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class RepresentedOrganisationDetailsMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly RepresentedOrganisationDetailsMap map;

        public RepresentedOrganisationDetailsMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = A.Fake<IMapper>();
            map = new RepresentedOrganisationDetailsMap(mapper);
        }

        [Fact]
        public void Map_WhenUseCurrentVersionIsFalse_ShouldMapAllProperties()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            source.UseCurrentVersion = false;
            var submissionData = source.SmallProducerSubmissionData;
            var fakeAddressData = fixture.Create<RepresentingCompanyAddressData>();

            A.CallTo(() => mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(
                    A<AuthorisedRepresentitiveData>.Ignored))
                .Returns(fakeAddressData);

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.Address.Should().Be(fakeAddressData);
            result.BusinessTradingName.Should()
                .Be(submissionData.CurrentSubmission.AuthorisedRepresentitiveData.BusinessTradingName);
            result.CompanyName.Should()
                .Be(submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CompanyName);
            result.RedirectToCheckAnswers.Should().Be(source.RedirectToCheckAnswers);

            A.CallTo(() => mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(
                submissionData.CurrentSubmission.AuthorisedRepresentitiveData)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_WhenUseCurrentVersionIsTrue_ShouldMapMasterProperties()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            source.UseCurrentVersion = true;
            var submissionData = source.SmallProducerSubmissionData;
            var fakeAddressData = fixture.Create<RepresentingCompanyAddressData>();

            A.CallTo(() => mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(
                    A<AuthorisedRepresentitiveData>.Ignored))
                .Returns(fakeAddressData);

            // Act
            var result = map.Map(source);

            // Assert
            result.Address.Should().Be(fakeAddressData);
            result.DirectRegistrantId.Should().Be(Guid.Empty);
            result.OrganisationId.Should().Be(Guid.Empty);
            result.BusinessTradingName.Should()
                .Be(source.SmallProducerSubmissionData.AuthorisedRepresentitiveData.BusinessTradingName);
            result.CompanyName.Should().Be(source.SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName);
            result.RedirectToCheckAnswers.Should().BeNull();

            A.CallTo(() => mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(
                submissionData.AuthorisedRepresentitiveData)).MustHaveHappenedOnceExactly();
        }
    }
}
