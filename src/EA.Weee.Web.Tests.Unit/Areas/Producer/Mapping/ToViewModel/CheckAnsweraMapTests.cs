namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class CheckAnswersMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly CheckAnswersMap map;

        public CheckAnswersMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();
            map = new CheckAnswersMap(mapper);
        }

        [Fact]
        public void Map_ShouldMapHighLevelSourceFields()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(submissionData.HasAuthorisedRepresentitive);
            result.ComplianceYear.Should().Be(source.Year ?? submissionData.CurrentSubmission.ComplianceYear);
        }

        [Fact]
        public void Map_ShouldMapServiceOfNotice()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedServiceOfNoticeModel = fixture.Create<ServiceOfNoticeViewModel>();
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, ServiceOfNoticeViewModel>(A<SubmissionsYearDetails>._))
                .Returns(expectedServiceOfNoticeModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.ServiceOfNoticeData.Should().Be(expectedServiceOfNoticeModel);
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, ServiceOfNoticeViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditOrganisationDetails()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedEditOrganisationDetailsModel = fixture.Create<OrganisationViewModel>();
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, OrganisationViewModel>(A<SubmissionsYearDetails>._))
                .Returns(expectedEditOrganisationDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.OrganisationDetails.Should().Be(expectedEditOrganisationDetailsModel);
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, OrganisationViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditContactDetails()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedContactDetailsModel = fixture.Create<ContactDetailsViewModel>();
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, ContactDetailsViewModel>(A<SubmissionsYearDetails>._))
                .Returns(expectedContactDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.Should().Be(expectedContactDetailsModel);
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, ContactDetailsViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditEeeDetails()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedEeeDataModel = fixture.Create<EditEeeDataViewModel>();
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, EditEeeDataViewModel>(A<SubmissionsYearDetails>._))
                .Returns(expectedEeeDataModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.EeeData.Should().Be(expectedEeeDataModel);
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, EditEeeDataViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapRepresentedCompanyDetails()
        {
            // Arrange
            var source = fixture.Create<SubmissionsYearDetails>();
            var submissionData = source.SmallProducerSubmissionData;
            submissionData.HasAuthorisedRepresentitive = true;
            var expectedRepresentedCompanyDetailsModel = fixture.Create<RepresentingCompanyDetailsViewModel>();
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>(A<SubmissionsYearDetails>._))
                .Returns(expectedRepresentedCompanyDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.RepresentingCompanyDetails.Should().Be(expectedRepresentedCompanyDetailsModel);
            A.CallTo(() => mapper.Map<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }
    }
}