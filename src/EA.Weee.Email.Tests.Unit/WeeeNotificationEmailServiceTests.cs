namespace EA.Weee.Email.Tests.Unit
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using EA.Prsd.Email;
    using FakeItEasy;
    using Xunit;

    public class WeeeNotificationEmailServiceTests
    {
        [Fact]
        public async Task SendOrganisationUserRequestCompleted_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeNotificationEmailServiceBuilder();
            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted(A<string>._, A<string>._, A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeMemberSubmitted.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeMemberSubmitted.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeNotificationEmailServiceBuilder();
            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted("a@b.com", A<string>._, A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeNotificationEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeMemberSubmitted(A<string>._, "TestSchemeName", A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "New member registration submission for TestSchemeName", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_SendsCreatedMailMessage()
        {
            // Arrange
            var builder = new WeeeNotificationEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted(A<string>._, A<string>._, A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.NotificationSender.SendAsync(mailMessage))
                .MustHaveHappened();
        }

        private class WeeeNotificationEmailServiceBuilder
        {
            public readonly ITemplateExecutor TemplateExecutor;
            public readonly IMessageCreator MessageCreator;
            public readonly INotificationSender NotificationSender;

            public WeeeNotificationEmailServiceBuilder()
            {
                TemplateExecutor = A.Fake<ITemplateExecutor>();
                MessageCreator = A.Fake<IMessageCreator>();
                NotificationSender = A.Fake<INotificationSender>();
            }

            public WeeeNotificationEmailService Build()
            {
                return new WeeeNotificationEmailService(TemplateExecutor, MessageCreator, NotificationSender);
            }
        }
    }
}
