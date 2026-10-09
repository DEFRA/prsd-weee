namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class DeleteAatfOrAeByIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly WeeeContext context;

        private readonly DeleteAatfOrAeByIdHandler handler;

        public DeleteAatfOrAeByIdHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            context = CreateContext();

            handler = new DeleteAatfOrAeByIdHandler(
                authorization,
                aatfDataAccess,
                context);
        }

        [Fact]
        public async Task HandleAsync_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            var command = CreateCommand();
            var aatf = CreateAatf();

            A.CallTo(() => aatfDataAccess.GetDetails(command.AatfId))
                                         .Returns(aatf);

            A.CallTo(() => aatfDataAccess.RemoveAatfRetenctionDataById(aatf))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.True(result);

            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                                        .MustHaveHappenedOnceExactly();

            A.CallTo(() => aatfDataAccess.GetDetails(command.AatfId))
                                         .MustHaveHappenedOnceExactly();

            A.CallTo(() => aatfDataAccess.RemoveAatfRetenctionDataById(aatf))
                                         .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenGetDetailsThrows_ReturnsFalse()
        {
            // Arrange
            var command = CreateCommand();

            A.CallTo(() => aatfDataAccess.GetDetails(command.AatfId))
                                         .Throws(new Exception("Get details failed"));

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result);

            A.CallTo(() => aatfDataAccess.RemoveAatfRetenctionDataById(A<Domain.AatfReturn.Aatf>._))
                                         .MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_WhenRemovalThrows_ReturnsFalse()
        {
            // Arrange
            var command = CreateCommand();
            var aatf = CreateAatf();

            A.CallTo(() => aatfDataAccess.GetDetails(command.AatfId))
                                         .Returns(aatf);

            A.CallTo(() => aatfDataAccess.RemoveAatfRetenctionDataById(aatf))
                                         .Throws(new Exception("Removal failed"));

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            Assert.False(result);

            A.CallTo(() => aatfDataAccess.GetDetails(command.AatfId))
                                         .MustHaveHappenedOnceExactly();

            A.CallTo(() => aatfDataAccess.RemoveAatfRetenctionDataById(aatf))
                                         .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenAuthorizationThrows_PropagatesException()
        {
            // Arrange
            var command = CreateCommand();

            A.CallTo(() => authorization.EnsureCanAccessInternalArea())
                                        .Throws(new UnauthorizedAccessException());

            // Act and assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(command));

            A.CallTo(() => aatfDataAccess.GetDetails(A<Guid>._))
                                         .MustNotHaveHappened();
        }

        private static DeleteAatfOrAeRecordById CreateCommand()
        {
            return new DeleteAatfOrAeRecordById(Guid.NewGuid());
        }

        private static Domain.AatfReturn.Aatf CreateAatf()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var originatingOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, originatingOrganisation);

                context.Aatfs.Add(aatf);

                db.WeeeContext.SaveChangesAsync();

                return aatf;
            }
        }

        private static WeeeContext CreateContext()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;
                return context;
            }
        }
    }
}
