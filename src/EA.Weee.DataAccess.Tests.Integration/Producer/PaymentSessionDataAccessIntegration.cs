namespace EA.Weee.DataAccess.Tests.Integration.Producer
{
    using Domain.Producer;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
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

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, year, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var validPaymentSession = CreatePaymentSession(user, year, directRegistrant, submission, paymentToken, "paymentId1", "paymentRef1");
                context.PaymentSessions.Add(validPaymentSession);

                // Add sessions that shouldn't be returned
                context.PaymentSessions.Add(CreatePaymentSession(user, year, directRegistrant, submission, "differentToken", "paymentId2", "paymentRef2", -1));
                context.PaymentSessions.Add(CreatePaymentSession(user, year, directRegistrant, submission, paymentToken, "paymentId3", "paymentRef3", -2));

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
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null, null);

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

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, year, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var finalStatePaymentSession = CreatePaymentSession(user, year, directRegistrant, submission, paymentToken, "paymentId", "paymentRef");
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

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, year, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var validPaymentSession = CreatePaymentSession(user, year, directRegistrant, submission, "testToken", "paymentId1", "paymentRef1", domainStatus);
                context.PaymentSessions.Add(validPaymentSession);

                // Add sessions that shouldn't be returned
                context.PaymentSessions.Add(CreatePaymentSession(notMatchedUser.Id, year, directRegistrant, submission, "token2", "paymentId2", "paymentRef2", domainStatus, -1)); // Different user
                context.PaymentSessions.Add(CreatePaymentSession(user, year + 1, directRegistrant, submission, "token3", "paymentId3", "paymentRef3", domainStatus, -2)); // Different year
                context.PaymentSessions.Add(CreatePaymentSession(user, year, directRegistrant, submission, "token4", "paymentId4", "paymentRef4", PaymentState.New, -3)); // Different state

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
                var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null, null);

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

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), SystemTime.UtcNow.Year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, SystemTime.UtcNow.Year, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var finalStatePaymentSession = CreatePaymentSession(user, year, directRegistrant, submission, "testToken", "paymentId", "paymentRef", PaymentState.Created);
                finalStatePaymentSession.InFinalState = true; // This should prevent it from being returned

                context.PaymentSessions.Add(finalStatePaymentSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentRetryPayment(directRegistrant.Id, year);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task AnyPaymentTokenAsync_WithExistingToken_ReturnsTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var(_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), SystemTime.UtcNow.Year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, SystemTime.UtcNow.Year, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const string paymentToken = "existingToken";
                var paymentSession = CreatePaymentSession(user, 2023, directRegistrant, submission, paymentToken, "paymentId1", "paymentRef1");
                context.PaymentSessions.Add(paymentSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.AnyPaymentTokenAsync(paymentToken);

                // Assert
                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task AnyPaymentTokenAsync_WithNonExistingToken_ReturnsFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const string paymentToken = "nonExistingToken";

                // Act
                var result = await dataAccess.AnyPaymentTokenAsync(paymentToken);

                // Assert
                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsPaymentSession()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), 2023);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, 2023, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var paymentSession = CreatePaymentSession(user, 2023, directRegistrant, submission, "token", "paymentId1", "paymentRef1");
                context.PaymentSessions.Add(paymentSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetByIdAsync(paymentSession.Id);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(paymentSession.Id);
                result.PaymentReturnToken.Should().Be("token");
                result.PaymentId.Should().Be("paymentId1");
                result.PaymentReference.Should().Be("paymentRef1");
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                var nonExistingId = Guid.NewGuid();

                // Act
                var result = await dataAccess.GetByIdAsync(nonExistingId);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetIncompletePaymentSessions_ReturnsValidIncompletePayments()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), 2023);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, 2023, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                // first payment session should be returned, older than 3 hours and not in final state
                var paymentSession1 = CreatePaymentSession(user, 2023, directRegistrant, submission, "token", "paymentId1", "paymentRef1", PaymentState.Started, -181, false);
                context.PaymentSessions.Add(paymentSession1);

                // second payment session should not be returned, younger than 3 hours and not in final state
                var paymentSession2 = CreatePaymentSession(user, 2023, directRegistrant, submission, "token", "paymentId1", "paymentRef1", PaymentState.New, -179, false);
                context.PaymentSessions.Add(paymentSession2);

                // third payment session should not be returned in final state
                var paymentSession3 = CreatePaymentSession(user, 2023, directRegistrant, submission, "token", "paymentId1", "paymentRef1", PaymentState.Success, -1, true);
                context.PaymentSessions.Add(paymentSession3);

                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetIncompletePaymentSessions(180, 10);

                // Assert
                result.Count.Should().BeGreaterOrEqualTo(2);
                result.Should().Contain(p => p.Id == paymentSession1.Id);
                result.Should().Contain(p => p.Id == paymentSession2.Id);
                result.Should().NotContain(p => p.Id == paymentSession3.Id);
            }
        }

        [Theory]
        [InlineData(PaymentStatus.Created)]
        [InlineData(PaymentStatus.Started)]
        [InlineData(PaymentStatus.Capturable)]
        [InlineData(PaymentStatus.New)]
        public async Task GetCurrentInProgressPayment_WithDifferentValidStates_ShouldReturnCorrectPaymentSession(PaymentStatus state)
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var domainStatus = state.ToDomainEnumeration<PaymentState>();
                var context = database.WeeeContext;
                var user = database.Model.AspNetUsers.First().Id;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));

                var dataAccess = new PaymentSessionDataAccess(context, userContext);

                const string paymentToken = "testToken";
                const int year = 2023;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, year,
                    new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                var validPaymentSession = CreatePaymentSession(user, year, directRegistrant, submission, paymentToken,
                    "paymentId1", "paymentRef1", domainStatus);
                context.PaymentSessions.Add(validPaymentSession);

                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentInProgressPayment(paymentToken, directRegistrant.Id, year);

                // Assert
                result.Should().NotBeNull();
                result.PaymentReturnToken.Should().Be(paymentToken);
                result.UserId.Should().Be(user);
                result.Status.Should().Be(domainStatus);
                result.InFinalState.Should().BeFalse();
                result.DirectProducerSubmission.ComplianceYear.Should().Be(year);
                result.DirectRegistrant.Should().NotBeNull();
                result.DirectRegistrant.Id.Should().Be(directRegistrant.Id);
                result.PaymentId.Should().Be("paymentId1");
                result.PaymentReference.Should().Be("paymentRef1");
            }
        }

        [Fact]
        public async Task GetCurrentInProgressPayment_WithInvalidState_ShouldReturnNull()
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

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "company",
                    SystemTime.UtcNow.Ticks.ToString(), year);

                var submission = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, year,
                    new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                // Create session with an invalid state (e.g., Success)
                var invalidSession = CreatePaymentSession(user, year, directRegistrant, submission, paymentToken,
                    "paymentId1", "paymentRef1", PaymentState.Success);

                context.PaymentSessions.Add(invalidSession);
                await context.SaveChangesAsync();

                // Act
                var result = await dataAccess.GetCurrentInProgressPayment(paymentToken, directRegistrant.Id, year);

                // Assert
                result.Should().BeNull();
            }
        }

        private static PaymentSession CreatePaymentSession(string userId, int year, DirectRegistrant directRegistrant, DirectProducerSubmission submission, string paymentToken, string paymentId, string paymentRef, int minutesOffset = 0)
        {
            return new PaymentSession(
                userId,
                100m,
                SystemTime.UtcNow.AddMinutes(minutesOffset),
                submission,
                directRegistrant,
                paymentToken,
                paymentId,
                paymentRef);
        }

        private static PaymentSession CreatePaymentSession(string userId, int year, DirectRegistrant directRegistrant, DirectProducerSubmission submission, string paymentToken, string paymentId, string paymentRef, PaymentState state, int minutesOffset = 0, bool inFinalState = false)
        {
            var session = new PaymentSession(
                userId,
                100m,
                SystemTime.UtcNow.AddMinutes(minutesOffset),
                submission,
                directRegistrant,
                paymentToken,
                paymentId,
                paymentRef)
            {
                Status = state,
                InFinalState = inFinalState
            };
            return session;
        }
    }
}