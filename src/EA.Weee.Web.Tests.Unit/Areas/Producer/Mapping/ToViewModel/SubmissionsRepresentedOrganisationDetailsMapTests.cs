namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class SubmissionsRepresentedOrganisationDetailsMapTests
    {
        private readonly IFixture fixture;
        private readonly SubmissionsRepresentedOrganisationDetailsMap map;
        private readonly IMapper mapper;

        public SubmissionsRepresentedOrganisationDetailsMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();

            map = new SubmissionsRepresentedOrganisationDetailsMap(mapper);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2024)]
        public void Map_ShouldMapValues(int? year)
        {
            // Arrange
            var producerSubmission = fixture.Create<SmallProducerSubmissionData>();
            producerSubmission.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
            {
                { year.HasValue ? year.Value : 2024, fixture.Create<SmallProducerSubmissionHistoryData>() }
            };

            producerSubmission.OrganisationData = fixture.Build<OrganisationData>().Create();
            producerSubmission.AuthorisedRepresentitiveData = fixture.Build<AuthorisedRepresentitiveData>().Create();

            var source = fixture.Build<SubmissionsYearDetails>()
            .With(x => x.SmallProducerSubmissionData, producerSubmission)
            .With(x => x.Year, year)
            .Create();

            var expectedAddress = fixture.Create<AddressPostcodeRequiredData>();
            A.CallTo(() => mapper.Map<AddressData, AddressPostcodeRequiredData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            RepresentingCompanyDetailsViewModel result = map.Map(source);

            // Assert
            if (year.HasValue)
            {
                result.DirectRegistrantId.Should().Be(source.SmallProducerSubmissionData.DirectRegistrantId);
                result.OrganisationId.Should().Be(source.SmallProducerSubmissionData.OrganisationData.Id);
                result.BusinessTradingName.Should().Be(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].AuthorisedRepresentitiveData.BusinessTradingName);
                result.CompanyName.Should().Be(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].AuthorisedRepresentitiveData.CompanyName);

                A.CallTo(() => mapper
                           .Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(source.SmallProducerSubmissionData.SubmissionHistory[year.Value].AuthorisedRepresentitiveData))
                           .MustHaveHappenedOnceExactly();
            }
            else
            {
                A.CallTo(() => mapper
                           .Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(A<SmallProducerSubmissionMapperData>.That
                           .Matches(x => x.SmallProducerSubmissionData == producerSubmission && x.UseMasterVersion == false)))
                           .MustHaveHappenedOnceExactly();
            }
        }
    }
}