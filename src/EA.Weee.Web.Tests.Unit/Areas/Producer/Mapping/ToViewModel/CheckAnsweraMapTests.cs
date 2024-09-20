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
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(source);

            // Assert
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.HasAuthorisedRepresentitive.Should().Be(submissionData.HasAuthorisedRepresentitive);
        }

        [Fact]
        public void Map_ShouldMapServiceOfNotice()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedServiceOfNoticeModel = fixture.Create<ServiceOfNoticeViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(A<SmallProducerSubmissionMapperData>._))
                .Returns(expectedServiceOfNoticeModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.ServiceOfNoticeData.Should().Be(expectedServiceOfNoticeModel);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditOrganisationDetails()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedEditOrganisationDetailsModel = fixture.Create<EditOrganisationDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>(A<SmallProducerSubmissionMapperData>._))
                .Returns(expectedEditOrganisationDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.OrganisationDetails.Should().Be(expectedEditOrganisationDetailsModel);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditContactDetails()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedContactDetailsModel = fixture.Create<EditContactDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>(A<SmallProducerSubmissionMapperData>._))
                .Returns(expectedContactDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.ContactDetails.Should().Be(expectedContactDetailsModel);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapEditEeeDetails()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedEeeDataModel = fixture.Create<EditEeeDataViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(A<SmallProducerSubmissionMapperData>._))
                .Returns(expectedEeeDataModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.EeeData.Should().Be(expectedEeeDataModel);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapRepresentedCompanyDetails()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            submissionData.HasAuthorisedRepresentitive = true;
            var expectedRepresentedCompanyDetailsModel = fixture.Create<RepresentingCompanyDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(A<SmallProducerSubmissionMapperData>._))
                .Returns(expectedRepresentedCompanyDetailsModel);

            // Act
            var result = map.Map(source);

            // Assert
            result.RepresentingCompanyDetails.Should().Be(expectedRepresentedCompanyDetailsModel);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(source))
                .MustHaveHappenedOnceExactly();
        }
    }
}