namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using AutoFixture;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class AddPaymentDetailsHandlerTests
    {
        private readonly AddPaymentDetailsHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IUserContext userContext;
        private readonly WeeeContext context;
        private readonly Fixture fixture;
        public AddPaymentDetailsHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            userContext = A.Fake<IUserContext>();
            dataAccess = A.Fake<IGenericDataAccess>();
            context = A.Fake<WeeeContext>();

            fixture = new Fixture();

            handler = new AddPaymentDetailsHandler(authorization, dataAccess, userContext, context);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            AddPaymentDetailsHandler handler = new AddPaymentDetailsHandler(authorization, dataAccess, userContext, context);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddPaymentDetails>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            AddPaymentDetailsHandler handler = new AddPaymentDetailsHandler(authorization, dataAccess, userContext, context);

            var req = this.fixture.Create<AddPaymentDetails>();

            Func<Task> action = async () => await handler.HandleAsync(req);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithPaymentFinished_ThrowsInvalidException()
        {
            //arrange
            var directProducerSubmissionId = Guid.NewGuid();

            var submission = A.Dummy<DirectProducerSubmission>();
            submission.PaymentFinished = true;

            var userContext = A.Fake<IUserContext>();

            var req = this.fixture.Build<AddPaymentDetails>().With(x => x.DirectProducerSubmissionId, directProducerSubmissionId).Create();

            A.CallTo(() => this.dataAccess.GetById<DirectProducerSubmission>(directProducerSubmissionId))
             .Returns(submission);

            //act
            AddPaymentDetailsHandler handler = new AddPaymentDetailsHandler(authorization, dataAccess, userContext, context);

            Func<Task> action = async () => await handler.HandleAsync(req);

            //assert
            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        public async Task HandleAsync_SetManualPaymentDetails()
        {
            //arrange
            var directProducerSubmissionId = Guid.NewGuid();

            var submission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => submission.DirectProducerSubmissionStatus).Returns(DirectProducerSubmissionStatus.Complete);

            var userContext = A.Fake<IUserContext>();

            var req = this.fixture.Build<AddPaymentDetails>().With(x => x.DirectProducerSubmissionId, directProducerSubmissionId).Create();

            A.CallTo(() => this.dataAccess.GetById<DirectProducerSubmission>(directProducerSubmissionId))
             .Returns(submission);

            //act
            var handlerResult = await handler.HandleAsync(req);

            //assert
            submission.ManualPaymentMadeByUserId.Should().Be(userContext.UserId.ToString());
            submission.ManualPaymentDetails.Should().Be(req.PaymentDetailsDescription);
            submission.ManualPaymentMethod.Should().Be(req.PaymentMethod);
            submission.ManualPaymentReceivedDate.Should().Be(req.PaymentRecievedDate.ToDateTime());
            submission.PaymentFinished.Should().BeTrue();

            A.CallTo(() => dataAccess.GetById<DirectProducerSubmission>(directProducerSubmissionId))
              .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => context.SaveChangesAsync())
              .MustHaveHappened(1, Times.Exactly);

            handlerResult.RegistrationNumber.Should().Be(submission.RegisteredProducer.ProducerRegistrationNumber);
            handlerResult.ComplianceYear.Should().Be(submission.ComplianceYear);
        }

        [Theory]
        [InlineData(SubmissionStatus.Returned)]
        [InlineData(SubmissionStatus.InComplete)]
        public void HandleAsync_WithSubmissionThatIsNotComplete_ShouldThrowInvalidOperationException(SubmissionStatus status)
        {
            //arrange
            var directProducerSubmissionId = Guid.NewGuid();
            var domainStatus = status.ToDomainEnumeration<DirectProducerSubmissionStatus>();

            var submission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => submission.DirectProducerSubmissionStatus).Returns(domainStatus);

            var req = fixture.Build<AddPaymentDetails>().With(x => x.DirectProducerSubmissionId, directProducerSubmissionId).Create();

            A.CallTo(() => dataAccess.GetById<DirectProducerSubmission>(directProducerSubmissionId))
                .Returns(submission);

            //act
            Func<Task> action = async () => await handler.HandleAsync(req);

            // Assert
            action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
