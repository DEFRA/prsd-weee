namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Email;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class SendTestEmailHandlerTests
    {
        /// <summary>
        /// This test ensures that a non-internal user cannot send a test email.
        /// </summary>
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async void HandleAsync_WithNonInternalUser_ThrowSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var emailService = A.Fake<IWeeeEmailService>();

            SendTestEmailHandler handler = new SendTestEmailHandler(authorization, emailService);

            // Act
            Func<Task<bool>> action = () => handler.HandleAsync(A.Dummy<SendTestEmail>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async void HandleAsync_SendEmailToSpecifiedAddress()
        {
            // Arrange
            var authorization = A.Fake<IWeeeAuthorization>();
            var emailService = A.Fake<IWeeeEmailService>();

            SendTestEmailHandler handler = new SendTestEmailHandler(authorization, emailService);

            // Act
            await handler.HandleAsync(new SendTestEmail("a@b.c"));

            // Assert
            A.CallTo(() => emailService.SendTestEmail("a@b.c"))
                .MustHaveHappened();
        }
    }
}
