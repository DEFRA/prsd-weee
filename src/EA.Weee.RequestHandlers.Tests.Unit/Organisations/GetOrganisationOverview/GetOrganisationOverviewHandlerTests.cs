namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.GetOrganisationOverview
{
    using EA.Weee.Requests.Organisations;
    using FakeItEasy;
    using RequestHandlers.Organisations.GetOrganisationOverview;
    using RequestHandlers.Organisations.GetOrganisationOverview.DataAccess;
    using RequestHandlers.Security;
    using System;
    using System.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetOrganisationOverviewHandlerTests
    {
        [Fact]
        public async void GetOrganisationOverviewHandler_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new GetOrganisationOverviewHandler(authorization, A.Dummy<IGetOrganisationOverviewDataAccess>());
            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<GetOrganisationOverview>()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetOrganisationOverviewHandler_ReturnsOrganisationOverview_WithHasMemberSubmissionsDetails(bool hasMemberSubmission)
        {           
            // Arrange
            var dataAccess = A.Fake<IGetOrganisationOverviewDataAccess>();
            A.CallTo(() => dataAccess.HasMemberSubmissions(A<Guid>._)).Returns(hasMemberSubmission);

            var handler = new GetOrganisationOverviewHandler(A.Dummy<IWeeeAuthorization>(), dataAccess);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetOrganisationOverview>());

            // Assert
            A.CallTo(() => dataAccess.HasMemberSubmissions(A<Guid>._)).MustHaveHappened();
            Assert.Equal(hasMemberSubmission, result.HasMemberSubmissions); 
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetOrganisationOverviewHandler_ReturnsOrganisationOverview_WithHasMultipleManageableOrganisationUsers(bool hasMultipleManageableUsers)
        {
            // Arrange
            var dataAccess = A.Fake<IGetOrganisationOverviewDataAccess>();
            A.CallTo(() => dataAccess.HasMultipleManageableOrganisationUsers(A<Guid>._)).Returns(hasMultipleManageableUsers);

            var handler = new GetOrganisationOverviewHandler(A.Dummy<IWeeeAuthorization>(), dataAccess);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetOrganisationOverview>());

            // Assert
            A.CallTo(() => dataAccess.HasMultipleManageableOrganisationUsers(A<Guid>._)).MustHaveHappened();
            Assert.Equal(hasMultipleManageableUsers, result.HasMultipleOrganisationUsers);
        }
    }
}
