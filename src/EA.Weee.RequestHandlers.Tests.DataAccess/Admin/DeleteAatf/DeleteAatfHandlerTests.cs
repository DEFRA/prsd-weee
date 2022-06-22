namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.DeleteAatf
{
    using AutoFixture;
    using Core.Admin;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Admin.Aatf;
    using RequestHandlers.Factories;
    using Requests.Admin.Aatf;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class DeleteAatfHandlerTests
    {
        private readonly IGetAatfDeletionStatus getAatfDeletionStatus;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly Fixture fixture;

        public DeleteAatfHandlerTests()
        {
            getAatfDeletionStatus = A.Fake<IGetAatfDeletionStatus>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            fixture = new Fixture();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfCannotBeDeleted_InvalidOperationExceptionExpectedAndAatfShouldNotBeDeleted()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var message = fixture.Create<DeleteAnAatf>();
                var organisation = EA.Weee.Domain.Organisation.Organisation.CreatePartnership("trading");
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().NotBeEmpty();

                var canAatfBeDeletedFlags = new CanAatfBeDeletedFlags();
                A.CallTo(() => getAatfDeletionStatus.Validate(message.AatfId)).Returns(canAatfBeDeletedFlags);

                var result = await Xunit.Record.ExceptionAsync(() => Handler(databaseWrapper).HandleAsync(message));

                result.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public async Task HandleAsync_GivenAatfCanBeDeleted_AatfShouldBeRemovedButOrganisationNotRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisation = EA.Weee.Domain.Organisation.Organisation.CreatePartnership("trading");
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().NotBeEmpty();

                var message = new DeleteAnAatf(aatf.Id, aatf.Organisation.Id);

                SetAatfDeleteFlags(message);

                await Handler(databaseWrapper).HandleAsync(message);

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Organisations.Where(a => a.Id == organisation.Id).Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task HandleAsync_GivenAatfAndOrganisationCanBeDeleted_AatfAndOrganisationShouldBeRemoved()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisation = EA.Weee.Domain.Organisation.Organisation.CreatePartnership("trading");
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.Organisations.Where(a => a.Id == organisation.Id).Should().NotBeEmpty();

                var message = new DeleteAnAatf(aatf.Id, aatf.Organisation.Id);

                SetOrganisationAndAatfDeleteFlags(message);

                await Handler(databaseWrapper).HandleAsync(message);

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().BeEmpty();
                databaseWrapper.WeeeContext.Organisations.Where(a => a.Id == organisation.Id).Should().BeEmpty();
            }
        }

        private DeleteAatfHandler Handler(DatabaseWrapper databaseWrapper)
        {
            return new DeleteAatfHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                new AatfDataAccess(databaseWrapper.WeeeContext, new GenericDataAccess(databaseWrapper.WeeeContext), quarterWindowFactory),
                new OrganisationDataAccess(databaseWrapper.WeeeContext),
                databaseWrapper.WeeeContext,
                getAatfDeletionStatus);
        }

        private void SetAatfDeleteFlags(DeleteAnAatf message)
        {
            var canAatfBeDeletedFlags = new CanAatfBeDeletedFlags();
            canAatfBeDeletedFlags |= CanAatfBeDeletedFlags.CanDelete;

            A.CallTo(() => getAatfDeletionStatus.Validate(message.AatfId)).Returns(canAatfBeDeletedFlags);
        }

        private void SetOrganisationAndAatfDeleteFlags(DeleteAnAatf message)
        {
            var canAatfBeDeletedFlags = new CanAatfBeDeletedFlags();
            canAatfBeDeletedFlags |= CanAatfBeDeletedFlags.CanDelete;
            canAatfBeDeletedFlags |= CanAatfBeDeletedFlags.CanDeleteOrganisation;

            A.CallTo(() => getAatfDeletionStatus.Validate(message.AatfId)).Returns(canAatfBeDeletedFlags);
        }
    }
}
