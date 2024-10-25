namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using EA.Weee.Domain.Producer;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Runtime.Remoting.Contexts;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class RemoveSmallProducerHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            var handler = new RemoveSmallProducerHandler(authorization, A.Dummy<IRemoveProducerDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<RemoveSmallProducer>()));
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new RemoveSmallProducerHandler(authorization, A.Dummy<IRemoveProducerDataAccess>());

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<RemoveSmallProducer>()));
        }

        [Fact]
        public async Task WhenUserIsUnauthorised_DoesNotGetProducer_OrSaveChanges()
        {
            // Arrange
            var builder = new RemoveSmallProducerHandlerBuilder();

            A.CallTo(() => builder.WeeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new RemoveSmallProducer(Guid.NewGuid());

            // Act
            await Assert.ThrowsAsync<SecurityException>(() => builder.Build().HandleAsync(request));

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerRegistration(A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => builder.RemoveProducerDataAccess.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task WhenUserIsAuthorised_GetsProducer_AndSaveChanges()
        {
            // Arrange
            var builder = new RemoveSmallProducerHandlerBuilder();
            var request = new RemoveSmallProducer(Guid.NewGuid());
            var producer = A.Fake<RegisteredProducer>();

            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerRegistration(request.RegisteredProducerId)).Returns(producer);

            // Act
            await builder.Build().HandleAsync(request);

            // Assert
            A.CallTo(() => builder.RemoveProducerDataAccess.GetProducerRegistration(request.RegisteredProducerId)).MustHaveHappened(1, Times.Exactly);

            producer.Removed.Should().BeTrue();

            A.CallTo(() => builder.RemoveProducerDataAccess.SaveChangesAsync())
                .MustHaveHappened(1, Times.Exactly);
        }

        private class RemoveSmallProducerHandlerBuilder
        {
            public readonly IWeeeAuthorization WeeeAuthorization;
            public readonly IRemoveProducerDataAccess RemoveProducerDataAccess;

            public RemoveSmallProducerHandlerBuilder()
            {
                WeeeAuthorization = A.Fake<IWeeeAuthorization>();
                RemoveProducerDataAccess = A.Fake<IRemoveProducerDataAccess>();
            }

            public RemoveSmallProducerHandler Build()
            {
                return new RemoveSmallProducerHandler(WeeeAuthorization, RemoveProducerDataAccess);
            }
        }
    }
}
