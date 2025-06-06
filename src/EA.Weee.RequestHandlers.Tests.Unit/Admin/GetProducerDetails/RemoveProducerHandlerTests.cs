﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using Domain.Charges;
    using Domain.Producer;
    using Domain.Scheme;
    using Domain.User;
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class RemoveProducerHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            var handler = new RemoveProducerHandler(authorization, A.Dummy<IRemoveProducerDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<RemoveProducer>()));
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new RemoveProducerHandler(authorization, A.Dummy<IRemoveProducerDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<RemoveProducer>()));
        }

        [Fact]
        public async Task WhenUserIsUnauthorised_DoesNotGetProducer_OrSaveChanges()
        {
            // Arrange
            var builder = new RemoveProducerHandlerBuilder();

            A.CallTo(() => builder.WeeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new RemoveProducer(Guid.NewGuid());

            // Act
            await Assert.ThrowsAsync<SecurityException>(() => builder.Build().HandleAsync(request));

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerRegistration(A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => builder.RemoveProducerDataAccess.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task WhenUserIsAuthorised_GetsProducer_AndProducerSubmissions_AndSaveChanges()
        {
            // Arrange
            var builder = new RemoveProducerHandlerBuilder();
            var request = new RemoveProducer(Guid.NewGuid());

            // Act
            await builder.Build().HandleAsync(request);

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerRegistration(request.RegisteredProducerId))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(request.RegisteredProducerId))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => builder.RemoveProducerDataAccess.SaveChangesAsync())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task DoesNotDeductChargeFromMemberUpload_WhenAlreadyInvoiced()
        {
            // Arrange
            var builder = new RemoveProducerHandlerBuilder();

            var memberUpload = new MemberUpload(A.Dummy<Guid>(), A.Dummy<string>(), null, 100, A.Dummy<int>(), A.Dummy<Scheme>(), A.Dummy<string>(), null, A.Dummy<bool>());
            memberUpload.Submit(A.Dummy<User>());
            memberUpload.AssignToInvoiceRun(A.Dummy<InvoiceRun>());

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.ChargeThisUpdate)
                .Returns(20);
            A.CallTo(() => producerSubmission.MemberUpload)
                .Returns(memberUpload);

            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(A<Guid>._))
                .Returns(new[] { producerSubmission });

            // Act
            await builder.Build().HandleAsync(A.Dummy<RemoveProducer>());

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(A<Guid>._))
                .MustHaveHappened();

            Assert.Equal(100, memberUpload.TotalCharges);
        }

        [Fact]
        public async Task DeductsChargeFromMemberUpload_WhenNotInvoiced()
        {
            // Arrange
            var builder = new RemoveProducerHandlerBuilder();

            var memberUpload = new MemberUpload(A.Dummy<Guid>(), A.Dummy<string>(), null, 100, A.Dummy<int>(), A.Dummy<Scheme>(), A.Dummy<string>(), null, A.Dummy<bool>());

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.ChargeThisUpdate)
                .Returns(20);
            A.CallTo(() => producerSubmission.MemberUpload)
                .Returns(memberUpload);

            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(A<Guid>._))
                .Returns(new[] { producerSubmission });

            // Act
            await builder.Build().HandleAsync(A.Dummy<RemoveProducer>());

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(A<Guid>._))
                .MustHaveHappened();

            Assert.Equal(80, memberUpload.TotalCharges);
        }

        private class RemoveProducerHandlerBuilder
        {
            public readonly IWeeeAuthorization WeeeAuthorization;
            public readonly IRemoveProducerDataAccess RemoveProducerDataAccess;

            public RemoveProducerHandlerBuilder()
            {
                WeeeAuthorization = A.Fake<IWeeeAuthorization>();
                RemoveProducerDataAccess = A.Fake<IRemoveProducerDataAccess>();
            }

            public RemoveProducerHandler Build()
            {
                return new RemoveProducerHandler(WeeeAuthorization, RemoveProducerDataAccess);
            }
        }
    }
}
