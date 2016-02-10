namespace EA.Weee.Email.Tests.Unit
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

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private class WeeeEmailServiceBuilder
        {
            public readonly ITemplateExecutor TemplateExecutor;
            public readonly IMessageCreator MessageCreator;
            public readonly ISender Sender;
            public readonly IWeeeEmailConfiguration Configuration;

            public WeeeEmailServiceBuilder()
            {
                TemplateExecutor = A.Fake<ITemplateExecutor>();
                MessageCreator = A.Fake<IMessageCreator>();
                Sender = A.Fake<ISender>();
                Configuration = A.Fake<IWeeeEmailConfiguration>();
            }

            public WeeeEmailService Build()
            {
                return new WeeeEmailService(TemplateExecutor, MessageCreator, Sender, Configuration);
            }
        }
    }
}
