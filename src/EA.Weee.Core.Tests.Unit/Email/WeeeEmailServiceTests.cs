namespace EA.Weee.Core.Tests.Unit.Email
{
    using EA.Prsd.Email;
    using EA.Weee.Email;
    using FakeItEasy;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Xunit;

    public class WeeeEmailServiceTests
    {
        [Fact]
        public async Task SendActivateUserAccount_HappyPath_ExecutesTemplatesCreatesMessageAndSendsMessage()
        {
            // Arrange
            string userEmailAddress = "someone@domain.com";
            string activationUrl = "http://";
            
            ITemplateExecutor templateExecutor = A.Fake<ITemplateExecutor>();
            IMessageCreator messageCreator = A.Fake<IMessageCreator>();
            ISender sender = A.Fake<ISender>();
            IWeeeEmailConfiguration configuration = A.Fake<IWeeeEmailConfiguration>();

            WeeeEmailService weeeEmailService = new WeeeEmailService(
                templateExecutor,
                messageCreator,
                sender,
                configuration);

            // Act
            await weeeEmailService.SendActivateUserAccount(userEmailAddress, activationUrl);

            // Assert
            A.CallTo(() => templateExecutor.Execute("ActivateUserAccount.cshtml", A<object>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(() => templateExecutor.Execute("ActivateUserAccount.txt", A<object>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => messageCreator.Create(userEmailAddress, A<string>._, A<EmailContent>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => sender.SendAsync(A<MailMessage>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
