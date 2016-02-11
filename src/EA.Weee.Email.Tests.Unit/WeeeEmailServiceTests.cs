﻿namespace EA.Weee.Email.Tests.Unit
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Prsd.Email;
    using Xunit;

    public class WeeeEmailServiceTests
    {
        [Fact]
        public async Task SendActivateUserAccount_HappyPath_ExecutesTemplatesCreatesMessageAndSendsMessage()
        {
            // Arrange
            string userEmailAddress = "someone@domain.com";
            string activationUrl = "http://";

            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendActivateUserAccount(userEmailAddress, activationUrl);

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("ActivateUserAccount.cshtml", A<object>._))
                .MustHaveHappened();

            A.CallTo(() => builder.TemplateExecutor.Execute("ActivateUserAccount.txt", A<object>._))
                .MustHaveHappened();

            A.CallTo(() => builder.MessageCreator.Create(userEmailAddress, A<string>._, A<EmailContent>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._, false))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
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
            var builder = new WeeeEmailServiceBuilder();
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
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeMemberSubmitted(A<string>._, "TestSchemeName", A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "New member registration submission for TestSchemeName", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted(A<string>._, A<string>._, A<int>._, A<int>._);

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, true))
                .MustHaveHappened();
        }

        private class WeeeEmailServiceBuilder
        {
            public readonly ITemplateExecutor TemplateExecutor;
            public readonly IMessageCreator MessageCreator;
            public readonly IWeeeSender Sender;
            public readonly IWeeeEmailConfiguration Configuration;

            public WeeeEmailServiceBuilder()
            {
                TemplateExecutor = A.Fake<ITemplateExecutor>();
                MessageCreator = A.Fake<IMessageCreator>();
                Sender = A.Fake<IWeeeSender>();
                Configuration = A.Fake<IWeeeEmailConfiguration>();
            }

            public WeeeEmailService Build()
            {
                return new WeeeEmailService(TemplateExecutor, MessageCreator, Sender, Configuration);
            }
        }
    }
}
