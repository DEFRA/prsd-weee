namespace EA.Weee.DataAccess.Tests.Integration.Producer
{
    using Domain.Producer;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PaymentSessionDataAccessIntegration
    {
        [Fact]
        public async Task GetCurrentInProgressPayment_WithValidInputs_ShouldReturnCorrectPaymentSession()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const string paymentToken = "testToken";
                const int year = 2023;

                var producer = new Domain.Producer.RegisteredProducer("ABC12345", SystemTime.UtcNow.Year);
                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("My company", "123456789");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                var validPaymentSession = CreatePaymentSession(user, year, producer, directRegistrant, paymentToken, "paymentId1", "paymentRef1");
                context.PaymentSessions.Add(validPaymentSession);

                // Add sessions that shouldn't be returned
                context.PaymentSessions.Add(CreatePaymentSession(user, year, producer, directRegistrant, "differentToken", "paymentId2", "paymentRef2", -1));
                context.PaymentSessions.Add(CreatePaymentSession(user, year, producer, directRegistrant, paymentToken, "paymentId3", "paymentRef3", -2));

                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentInProgressPayment(paymentToken, directRegistrant.Id, year);

                // Assert
                result.Should().NotBeNull();
                result.PaymentReturnToken.Should().Be(paymentToken);
                result.UserId.Should().Be(user);
                result.Status.Should().Be(PaymentState.New);
                result.InFinalState.Should().BeFalse();
                result.DirectProducerSubmission.ComplianceYear.Should().Be(year);
                result.DirectRegistrant.Should().NotBeNull();
                result.DirectRegistrant.Id.Should().Be(directRegistrant.Id);
                result.PaymentId.Should().Be("paymentId1");
                result.PaymentReference.Should().Be("paymentRef1");
            }
        }

        [Fact]
        public async Task GetCurrentInProgressPayment_WithNoMatchingPaymentSession_ShouldReturnNull()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("Test Company", "987654321");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                // Act
                var result = await dataAccess.GetCurrentInProgressPayment("nonExistentToken", directRegistrant.Id, 2023);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetCurrentInProgressPayment_WithFinalStatePaymentSession_ShouldReturnNull()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const string paymentToken = "testToken";
                const int year = 2023;

                var producer = new Domain.Producer.RegisteredProducer("ABC12345", SystemTime.UtcNow.Year);
                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("My company", "123456789");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                var finalStatePaymentSession = CreatePaymentSession(user, year, producer, directRegistrant, paymentToken, "paymentId", "paymentRef");
                finalStatePaymentSession.InFinalState = true; // This should prevent it from being returned

                context.PaymentSessions.Add(finalStatePaymentSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentInProgressPayment(paymentToken, directRegistrant.Id, year);

                // Assert
                result.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(PaymentStatus.Created)]
        [InlineData(PaymentStatus.Started)]
        [InlineData(PaymentStatus.Submitted)]
        [InlineData(PaymentStatus.New)]
        public async Task GetCurrentRetryPayment_WithValidInputs_ShouldReturnCorrectPaymentSession(PaymentStatus state)
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var domainStatus = state.ToDomainEnumeration<PaymentState>();
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var notMatchedUser = new AspNetUser
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Test",
                    Surname = "LastName",
                    Email = "a@b.c",
                    EmailConfirmed = true,
                    UserName = "user@email.com"
                };

                database.Model.AspNetUsers.Add(notMatchedUser);
                database.Model.SaveChanges();

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const int year = 2023;

                var producer = new Domain.Producer.RegisteredProducer("ABC12345", year);
                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("My company", "123456789");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                var validPaymentSession = CreatePaymentSession(user, year, producer, directRegistrant, "testToken", "paymentId1", "paymentRef1", domainStatus);
                context.PaymentSessions.Add(validPaymentSession);

                // Add sessions that shouldn't be returned
                context.PaymentSessions.Add(CreatePaymentSession(notMatchedUser.Id, year, producer, directRegistrant, "token2", "paymentId2", "paymentRef2", domainStatus, -1)); // Different user
                context.PaymentSessions.Add(CreatePaymentSession(user, year + 1, producer, directRegistrant, "token3", "paymentId3", "paymentRef3", domainStatus, -2)); // Different year
                context.PaymentSessions.Add(CreatePaymentSession(user, year, producer, directRegistrant, "token4", "paymentId4", "paymentRef4", PaymentState.New, -3)); // Different state

                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentRetryPayment(directRegistrant.Id, year);

                // Assert
                result.Should().NotBeNull();
                result.UserId.Should().Be(user);
                result.Status.Should().Be(domainStatus);
                result.InFinalState.Should().BeFalse();
                result.DirectProducerSubmission.ComplianceYear.Should().Be(year);
                result.DirectRegistrant.Should().NotBeNull();
                result.DirectRegistrantId.Should().Be(directRegistrant.Id);
                result.PaymentId.Should().Be("paymentId1");
                result.PaymentReference.Should().Be("paymentRef1");
            }
        }

        [Fact]
        public async Task GetCurrentRetryPayment_WithNoMatchingPaymentSession_ShouldReturnNull()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("Test Company", "987654321");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                // Act
                var result = await dataAccess.GetCurrentRetryPayment(directRegistrant.Id, 2023);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetCurrentRetryPayment_WithFinalStatePaymentSession_ShouldReturnNull()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const int year = 2023;

                var producer = new Domain.Producer.RegisteredProducer("ABC12345", year);
                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("My company", "123456789");
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);

                var finalStatePaymentSession = CreatePaymentSession(user, year, producer, directRegistrant, "testToken", "paymentId", "paymentRef", PaymentState.Created);
                finalStatePaymentSession.InFinalState = true; // This should prevent it from being returned

                context.PaymentSessions.Add(finalStatePaymentSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentRetryPayment(directRegistrant.Id, year);

                // Assert
                result.Should().BeNull();
            }
        }

        private static PaymentSession CreatePaymentSession(string userId, int year, Domain.Producer.RegisteredProducer producer, DirectRegistrant directRegistrant, string paymentToken, string paymentId, string paymentRef, int minutesOffset = 0)
        {
            return new PaymentSession(
                userId,
                100m,
                SystemTime.UtcNow.AddMinutes(minutesOffset),
                new DirectProducerSubmission { ComplianceYear = year, RegisteredProducer = producer },
                directRegistrant,
                paymentToken,
                paymentId,
                paymentRef);
        }

        private static PaymentSession CreatePaymentSession(string userId, int year, Domain.Producer.RegisteredProducer producer, DirectRegistrant directRegistrant, string paymentToken, string paymentId, string paymentRef, PaymentState state, int minutesOffset = 0)
        {
            var session = new PaymentSession(
                userId,
                100m,
                SystemTime.UtcNow.AddMinutes(minutesOffset),
                new DirectProducerSubmission { ComplianceYear = year, RegisteredProducer = producer },
                directRegistrant,
                paymentToken,
                paymentId,
                paymentRef)
            {
                Status = state
            };
            return session;
        }
    }
}