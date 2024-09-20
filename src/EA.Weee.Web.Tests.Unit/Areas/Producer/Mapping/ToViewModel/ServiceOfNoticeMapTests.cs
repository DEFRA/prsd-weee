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

    public class ServiceOfNoticeMapTests
    {
        private readonly IFixture fixture;
        private readonly IMapper mapper;
        private readonly ServiceOfNoticeMap map;

        public ServiceOfNoticeMapTests()
        {
            fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            mapper = fixture.Freeze<IMapper>();
            map = new ServiceOfNoticeMap(mapper);
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
        public void Map_ShouldMapServiceOfNoticeAddress()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            var expectedAddress = fixture.Create<ServiceOfNoticeAddressData>();
            A.CallTo(() => mapper.Map<AddressData, ServiceOfNoticeAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.Address.Should().Be(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, ServiceOfNoticeAddressData>(submissionData.CurrentSubmission.ServiceOfNoticeData))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_ShouldMapNullServiceOfNoticeAddress()
        {
            // Arrange
            var source = fixture.Create<SmallProducerSubmissionMapperData>();
            var submissionData = source.SmallProducerSubmissionData;
            submissionData.CurrentSubmission.ServiceOfNoticeData = null;
            var expectedAddress = new ServiceOfNoticeAddressData();
            A.CallTo(() => mapper.Map<AddressData, ServiceOfNoticeAddressData>(A<AddressData>._))
                .Returns(expectedAddress);

            // Act
            var result = map.Map(source);

            // Assert
            result.Address.Should().Be(expectedAddress);
            A.CallTo(() => mapper.Map<AddressData, ServiceOfNoticeAddressData>(submissionData.CurrentSubmission.ServiceOfNoticeData))
                .MustHaveHappenedOnceExactly();
        }
    }
}