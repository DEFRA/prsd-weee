namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Scheme.GetSchemePublicInfo;
    using Requests.Scheme;
    using System;
    using Core.Helpers;
    using Xunit;

    public class GetSchemePublicInfoHandlerTests
    {
        [Fact]
        public async void HandleAsync_HappyPath_ReturnsSchemeData()
        {
            // Arrange
            var dataAccess = A.Fake<IGetSchemePublicInfoDataAccess>();
            var scheme = A.Fake<Scheme>();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => scheme.Id).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.SchemeName).Returns("scheme");
            A.CallTo(() => scheme.ApprovalNumber).Returns("approval");
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);
            A.CallTo(() => dataAccess.FetchSchemeByOrganisationId(organisationId)).Returns(scheme);

            var handler = new GetSchemePublicInfoHandler(dataAccess);
            var request = new GetSchemePublicInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.OrganisationId.Should().Be(scheme.OrganisationId);
            result.Name.Should().Be(scheme.SchemeName);
            result.ApprovalNo.Should().Be(scheme.ApprovalNumber);
            result.SchemeId.Should().Be(scheme.Id);
            result.Status.Should().Be(Core.Shared.SchemeStatus.Approved);
            result.StatusName.Should().Be(Core.Shared.SchemeStatus.Approved.ToDisplayString());
        }
    }
}
