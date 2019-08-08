namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DeleteAatf
{
    using EA.Weee.Core.Admin;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Admin.DeleteAatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using RequestHandlers.Admin.DeleteAatf.DeleteValidation;
    using Xunit;

    public class CheckAatfCanBeDeletedHandlerTests
    {
        private readonly IGetAatfDeletionStatus aatfDeletionStatus;
        private readonly IGetOrganisationDeletionStatus organisationDeletionStatus;
        private readonly CheckAatfCanBeDeletedHandler handler;
        private readonly IAatfDataAccess aatfDataAccess;

        public CheckAatfCanBeDeletedHandlerTests()
        {
            aatfDeletionStatus = A.Fake<IGetAatfDeletionStatus>();
            organisationDeletionStatus = A.Fake<IGetOrganisationDeletionStatus>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();

            handler = new CheckAatfCanBeDeletedHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                aatfDeletionStatus,
                organisationDeletionStatus,
                aatfDataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var handler = new CheckAatfCanBeDeletedHandler(AuthorizationBuilder.CreateFromUserType(userType), aatfDeletionStatus, organisationDeletionStatus, aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var handler = new CheckAatfCanBeDeletedHandler(new AuthorizationBuilder().DenyRole(Roles.InternalAdmin).Build(), aatfDeletionStatus, organisationDeletionStatus, aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }
    }
}
