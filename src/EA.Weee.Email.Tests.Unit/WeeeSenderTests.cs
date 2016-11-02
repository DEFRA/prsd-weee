namespace EA.Weee.Email.Tests.Unit
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Prsd.Email;
    using Serilog;
    using Serilog.Events;
    using Xunit;
    
    public class WeeeSenderTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SendAsync_SendsSpecifiedMailMessage(bool continueOnException)
        {
            // Arrange
            var builder = new WeeeSenderBuilder();
            var weeeSender = builder.Build();
            var mailMessage = new MailMessage();

            // Act
            await weeeSender.SendAsync(mailMessage, continueOnException);

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(mailMessage))
                .MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_SenderThrowsException_FalseForContinueOnException_ExceptionIsNotThrown()
        {
            // Arrange
            var builder = new WeeeSenderBuilder();

            var senderException = new Exception();
            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                 .Throws(senderException);

            var weeeSender = builder.Build();

            // Act, Assert
            var thrownException = await Record.ExceptionAsync(() => weeeSender.SendAsync(A.Dummy<MailMessage>(), false));

            Assert.NotNull(thrownException);
            Assert.Same(senderException, thrownException);
        }

        [Fact]
        public async Task SendAsync_SenderThrowsException_TrueForContinueOnException_ExceptionIsLogged()
        {
            // Arrange
            var builder = new WeeeSenderBuilder();

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                 .Throws(new Exception());

            var weeeSender = builder.Build();

            // Act
            var result = await weeeSender.SendAsync(A.Dummy<MailMessage>(), true);

            // Assert
            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                .MustHaveHappened();

            A.CallTo(() => builder.Logger.Write(LogEventLevel.Error, A<Exception>._, A<string>._))
                .MustHaveHappened();

            Assert.True(result);
        }

        [Fact]
        public async Task SendAsync_SenderThrowsException_TrueForContinueOnException_LogContainsExceptionThrownAndMessageSubject()
        {
            // Arrange
            var builder = new WeeeSenderBuilder();

            var senderException = new Exception();
            var message = new MailMessage
            {
                Subject = "TestEmail"
            };

            A.CallTo(() => builder.Sender.SendAsync(A<MailMessage>._))
                 .Throws(senderException);

            Exception generatedException = null;

            A.CallTo(() => builder.Logger.Write(LogEventLevel.Error, A<Exception>._, A<string>._))
                .Invokes((LogEventLevel l, Exception x, String m) => generatedException = x);

            var weeeSender = builder.Build();

            // Act
            var result = await weeeSender.SendAsync(message, true);

            // Assert
            Assert.NotNull(generatedException);
            Assert.Contains("'TestEmail'", generatedException.Message);
            Assert.Same(senderException, generatedException.InnerException);
            Assert.True(result);
        }

        private class WeeeSenderBuilder
        {
            public readonly ISender Sender;
            public readonly ILogger Logger;

            public WeeeSenderBuilder()
            {
                Sender = A.Fake<ISender>();
                Logger = A.Fake<ILogger>();
            }

            public WeeeSender Build()
            {
                return new WeeeSender(Sender, Logger);
            }
        }
    }
}
