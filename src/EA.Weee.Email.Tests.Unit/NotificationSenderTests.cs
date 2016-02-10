namespace EA.Weee.Email.Tests.Unit
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Core.Logging;
    using FakeItEasy;
    using Prsd.Email;
    using Xunit;

    public class NotificationSenderTests
    {
        [Fact]
        public async Task SendAsync_SendsSpecifiedMailMessage()
        {
            // Arrange
            var builder = new NotificationSenderBuilder();
            var notificationSender = builder.Build();
            var mailMessage = new MailMessage();

            // Act
            await notificationSender.SendAsync(mailMessage);

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_SenderThrowsException_ExceptionIsLogged()
        {
            // Arrange
            var builder = new NotificationSenderBuilder();

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                 .Throws(new Exception());

            var notificationSender = builder.Build();

            // Act
            await notificationSender.SendAsync(A.Dummy<MailMessage>());

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                .MustHaveHappened();

            A.CallTo(() => builder.Logger.Log(A<Exception>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_SenderThrowsException_LogContainsExceptionThrownAndMessageSubject()
        {
            // Arrange
            var builder = new NotificationSenderBuilder();

            var senderException = new Exception();
            var message = new MailMessage
            {
                Subject = "TestEmail"
            };

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                 .Throws(senderException);

            Exception generatedException = null;

            A.CallTo(() => builder.Logger.Log(A<Exception>._))
                .Invokes((Exception x) => generatedException = x);

            var notificationSender = builder.Build();

            // Act
            await notificationSender.SendAsync(message);

            // Assert
            Assert.NotNull(generatedException);
            Assert.Contains("'TestEmail'", generatedException.Message);
            Assert.Same(senderException, generatedException.InnerException);
        }

        private class NotificationSenderBuilder
        {
            public readonly ISender Sender;
            public readonly ILogger Logger;

            public NotificationSenderBuilder()
            {
                Sender = A.Fake<ISender>();
                Logger = A.Fake<ILogger>();
            }

            public NotificationSender Build()
            {
                return new NotificationSender(Sender, Logger);
            }
        }
    }
}
