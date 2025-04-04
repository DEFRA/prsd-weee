namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DirectRegistrants
{
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Admin.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class ReturnSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly WeeeContext context;
        private readonly ReturnSmallProducerSubmissionHandler handler;
        private readonly Guid submissionId;
        private readonly DirectProducerSubmission submission;
        private readonly RegisteredProducer registeredProducer;
        private readonly DirectProducerSubmissionHistory currentSubmission;

        public ReturnSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            smallProducerDataAccess = A.Fake<ISmallProducerDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            context = A.Fake<WeeeContext>();
            submissionId = Guid.NewGuid();

            // Create base dependencies
            registeredProducer = A.Fake<RegisteredProducer>();

            // Create the main submission
            submission = new DirectProducerSubmission(registeredProducer, DateTime.Now.Year)
            {
                DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete
            };

            // Create and configure current submission
            currentSubmission = new DirectProducerSubmissionHistory(submission)
            {
                CompanyName = "Test Company",
                TradingName = "Test Trading"
            };

            submission.SetCurrentSubmission(currentSubmission);

            handler = new ReturnSmallProducerSubmissionHandler(
                authorization,
                smallProducerDataAccess,
                context,
                systemDataDataAccess);

            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionById(submissionId))
                .Returns(submission);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime())
                .Returns(new DateTime(DateTime.Now.Year, 1, 1));
        }

        [Fact]
        public void HandleAsync_GivenNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task> act = async () => await handler.HandleAsync(null);

            // Act & Assert
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_ChecksInternalAreaAccess()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_ChecksInternalUserRole()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureUserInRole(Roles.InternalAdmin))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleAsync_WhenUnauthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            Func<Task> act = async () => await handler.HandleAsync(request);

            // Act & Assert
            act.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_WhenSubmissionNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            A.CallTo(() => smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionById(submissionId))
                .Returns<DirectProducerSubmission>(null);

            Func<Task> act = async () => await handler.HandleAsync(request);

            // Act & Assert
            (await act.Should().ThrowAsync<InvalidOperationException>())
                .WithMessage("Submission status invalid");
        }

        [Fact]
        public async Task HandleAsync_WhenSubmissionNotComplete_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            submission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;

            Func<Task> act = async () => await handler.HandleAsync(request);

            // Act & Assert
            (await act.Should().ThrowAsync<InvalidOperationException>())
                .WithMessage("Submission status invalid");
        }

        [Fact]
        public async Task HandleAsync_CreatesNewSubmissionHistory()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => context.DirectProducerSubmissionHistories.Add(A<DirectProducerSubmissionHistory>.That.Matches(h =>
                        h.CompanyName == currentSubmission.CompanyName &&
                        h.TradingName == currentSubmission.TradingName &&
                        h.SellingTechniqueType == currentSubmission.SellingTechniqueType)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CopiesAddresses()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            var address = new Address("Address1", "Address2", "Town", "County", "PostCode", A.Fake<Country>(), "123", "test@test.com");
            currentSubmission.AddOrUpdateBusinessAddress(address);
            currentSubmission.AddOrUpdateContactAddress(address);
            currentSubmission.AddOrUpdateServiceOfNotice(address);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => context.DirectProducerSubmissionHistories.Add(A<DirectProducerSubmissionHistory>.That.Matches(h =>
                        h.BusinessAddress != null &&
                        h.ContactAddress != null &&
                        h.ServiceOfNoticeAddress != null &&
                        h.BusinessAddress.Address1 == address.Address1 &&
                        h.BusinessAddress.Address2 == address.Address2 &&
                        h.BusinessAddress.TownOrCity == address.TownOrCity &&
                        h.BusinessAddress.CountyOrRegion == address.CountyOrRegion &&
                        h.BusinessAddress.Postcode == address.Postcode &&
                        h.BusinessAddress.Country == address.Country)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CopiesContacts()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            var contact = new Contact("John", "Doe", "Manager");
            currentSubmission.AddOrUpdateContact(contact);
            currentSubmission.AddOrUpdateAppropriateSignatory(contact);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => context.DirectProducerSubmissionHistories.Add(A<DirectProducerSubmissionHistory>.That.Matches(h =>
                        h.Contact != null &&
                        h.AppropriateSignatory != null &&
                        h.Contact.FirstName == contact.FirstName &&
                        h.Contact.LastName == contact.LastName &&
                        h.Contact.Position == contact.Position)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CopiesEeeOutputData()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            var eeeOutputVersion = new EeeOutputReturnVersion();
            var eeeOutputAmount = new EeeOutputAmount(
                ObligationType.B2B,
                EA.Weee.Domain.Lookup.WeeeCategory.ConsumerEquipment,
                1.5m,
                registeredProducer);
            eeeOutputVersion.EeeOutputAmounts.Add(eeeOutputAmount);
            currentSubmission.EeeOutputReturnVersion = eeeOutputVersion;

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => context.DirectProducerSubmissionHistories.Add(
                    A<DirectProducerSubmissionHistory>.That.Matches(h =>
                        h.EeeOutputReturnVersion != null &&
                        h.EeeOutputReturnVersion.EeeOutputAmounts.Count == 1 &&
                        h.EeeOutputReturnVersion.EeeOutputAmounts.First().ObligationType == eeeOutputAmount.ObligationType &&
                        h.EeeOutputReturnVersion.EeeOutputAmounts.First().WeeeCategory == eeeOutputAmount.WeeeCategory &&
                        h.EeeOutputReturnVersion.EeeOutputAmounts.First().Tonnage == eeeOutputAmount.Tonnage)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_UpdatesSubmissionStatus()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Returned);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(2, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_ReturnsSubmissionId()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().Be(submission.Id);
        }

        [Fact]
        public async Task HandleAsync_WhenSubmissionYearDoesNotMatchSystemYear_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);

            var nextYear = DateTime.Now.Year + 1;
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime())
                .Returns(new DateTime(nextYear, 1, 1));

            // Act
            Func<Task> act = async () => await handler.HandleAsync(request);

            // Assert
            (await act.Should().ThrowAsync<InvalidOperationException>())
                .WithMessage("Submission status invalid");
        }

        [Fact]
        public async Task HandleAsync_WhenSubmissionYearMatchesSystemYear_Succeeds()
        {
            // Arrange
            var request = new ReturnSmallProducerSubmission(submissionId);
            var currentYear = DateTime.Now.Year;

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime())
                .Returns(new DateTime(currentYear, 1, 1));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().Be(submission.Id);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened();
            submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Returned);
        }
    }
}