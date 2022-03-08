namespace EA.Weee.Email.Tests.Unit
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using Prsd.Email;
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using EA.Weee.Core.Shared;

    using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;

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
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._, false))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task SendSchemeMemberSubmitted_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeMemberSubmitted.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeMemberSubmitted.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeMemberSubmitted_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted("a@b.com", A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeMemberSubmitted_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeMemberSubmitted(A.Dummy<string>(), "TestSchemeName", A.Dummy<int>(), A.Dummy<int>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "Member registration submission for TestSchemeName", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeMemberSubmitted_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var notificationService = builder.Build();

            // Act
            await notificationService.SendSchemeMemberSubmitted(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, true))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeDataReturnSubmitted_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeDataReturnSubmitted(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>(), A.Dummy<bool>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeDataReturnSubmitted.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("SchemeDataReturnSubmitted.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeDataReturnSubmitted_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeDataReturnSubmitted(
                "a@b.com", A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>(), A.Dummy<bool>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeDataReturnSubmitted_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeDataReturnSubmitted(
                A.Dummy<string>(), "TestSchemeName", A.Dummy<int>(), A.Dummy<int>(), A.Dummy<bool>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "Data return submission for TestSchemeName", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendSchemeDataReturnSubmitted_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var emailService = builder.Build();

            // Act
            await emailService.SendSchemeDataReturnSubmitted(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<int>(), A.Dummy<int>(), A.Dummy<bool>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, true))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendInternalUserAccountActivated_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendInternalUserAccountActivated(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("InternalUserAccountActivated.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("InternalUserAccountActivated.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendInternalUserAccountActivated_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendInternalUserAccountActivated(
                "a@b.com", A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendInternalUserAccountActivated_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendInternalUserAccountActivated(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "New internal user request", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendInternalUserAccountActivated_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var emailService = builder.Build();

            // Act
            await emailService.SendInternalUserAccountActivated(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, true))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendTestEmail_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendTestEmail(A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("Test.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("Test.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendTestEmail_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendTestEmail("a@b.com");

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendTestEmail_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendTestEmail(A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "Test email from WEEE", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendTestEmail_SendsCreatedMailMessageWithThrowOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var emailService = builder.Build();

            // Act
            await emailService.SendTestEmail(A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, false))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationContactDetailsChanged_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationContactDetailsChanged(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<EntityType>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationContactDetailsChanged.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationContactDetailsChanged.txt", A<object>._))
                .MustHaveHappened();
        }

        [Theory]
        [InlineData(EntityType.Aatf)]
        [InlineData(EntityType.Ae)]
        [InlineData(EntityType.Pcs)]
        public async Task SendOrganisationContactDetailsChanged_InvokesExecutorWithCorrectModel(EntityType entityType)
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationContactDetailsChanged(A.Dummy<string>(), "NAME", entityType);

            // Assert
            A.CallTo(
                    () => builder.TemplateExecutor.Execute(
                        "OrganisationContactDetailsChanged.cshtml",
                        A<object>.That.Matches(
                            a => a.GetPropertyValue<string>("Name") == "NAME" && a.GetPropertyValue<EntityType>("EntityType") == entityType)))
                .MustHaveHappened();
            A.CallTo(
                    () => builder.TemplateExecutor.Execute(
                        "OrganisationContactDetailsChanged.txt",
                        A<object>.That.Matches(
                            a => a.GetPropertyValue<string>("Name") == "NAME" && a.GetPropertyValue<EntityType>("EntityType") == entityType)))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationContactDetailsChanged_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationContactDetailsChanged("a@b.com", A.Dummy<string>(), A.Dummy<EntityType>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create("a@b.com", A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationContactDetailsChanged_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationContactDetailsChanged(A.Dummy<string>(), "TestPCS", A.Dummy<EntityType>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A.Dummy<string>(), "Change of contact details for TestPCS", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationContactDetailsChanged_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            // Act
            await emailService.SendOrganisationContactDetailsChanged(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<EntityType>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, true))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequest_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var email = "email@civica.co.uk";
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>.That.IsEqualTo(email), A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            // Act
            await emailService.SendOrganisationUserRequest(email, "TEST", "TEST");

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, false))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestToEA_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationUserRequestToEA(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A.Dummy<string>(), "New request to access an organisation in WEEE Online", A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestToEA_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();
            var email = "test@civica.co.uk";

            // Act
            await emailService.SendOrganisationUserRequestToEA(email, A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(email, A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestToEA_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationUserRequestToEA(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationUserRequestToEA.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationUserRequestToEA.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestToEA_SendsCreatedMailMessageWithContinueOnException()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var mailMessage = new MailMessage();

            A.CallTo(() => builder.MessageCreator.Create(A<string>._, A<string>._, A<EmailContent>._))
                .Returns(mailMessage);

            var emailService = builder.Build();

            // Act
            await emailService.SendOrganisationUserRequestToEA(
                A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage, false)).MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_InvokesExecutorWithCorrectTemplateNames()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var notificationService = builder.Build();

            // Act
            await notificationService.SendOrganisationUserRequestCompleted(A.Fake<OrganisationUser>(), true);

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationUserRequestCompleted.cshtml", A<object>._))
                .MustHaveHappened();
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationUserRequestCompleted.txt", A<object>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_CreatesMailMessageWithSpecifiedEmailAddress()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var notificationService = builder.Build();

            var organisationUser = A.Fake<OrganisationUser>();

            // Act
            await notificationService.SendOrganisationUserRequestCompleted(organisationUser, true);

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(organisationUser.User.Email, A<string>._, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_CreatesMailMessageWithCorrectSubject()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            var organisationUser = A.Fake<OrganisationUser>();

            // Act
            await emailService.SendOrganisationUserRequestCompleted(organisationUser, true);

            // Assert
            A.CallTo(() => builder.MessageCreator.Create(A<string>._, "Your request to access " + organisationUser.Organisation.OrganisationName, A<EmailContent>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendOrganisationUserRequestCompleted_ModelIsBuiltWithCorrectParameters()
        {
            // Arrange
            var builder = new WeeeEmailServiceBuilder();
            var emailService = builder.Build();

            var organisationUser = A.Fake<OrganisationUser>();

            // Act
            await emailService.SendOrganisationUserRequestCompleted(organisationUser, true);

            // Assert
            A.CallTo(() => builder.TemplateExecutor.Execute("OrganisationUserRequestCompleted.cshtml", A<object>.That.Matches(m => m.GetPropertyValue<bool>("ActiveUsers") == true
            && m.GetPropertyValue<string>("FullName") == organisationUser.User.FullName
            && m.GetPropertyValue<string>("OrganisationName") == organisationUser.Organisation.OrganisationName
            && m.GetPropertyValue<bool>("Approved") == false
            && m.GetPropertyValue<string>("ExternalUserLoginUrl") == builder.Configuration.ExternalUserLoginUrl)))
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
