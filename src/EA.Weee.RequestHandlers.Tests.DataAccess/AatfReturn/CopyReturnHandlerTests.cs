namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;
    using ReturnStatus = Domain.AatfReturn.ReturnStatus;

    public class CopyReturnHandlerTests
    {
        private CopyReturnHandler handler;
        private DatabaseWrapper database;
        private Return @return;
        private Return copiedReturn;
        private Organisation organisation;
        private Guid returnId;

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new CopyReturnHandler(authorization,
                A.Dummy<WeeeContext>(),
                A.Dummy<IUserContext>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CopyReturn>());

            await action.Should().ThrowAsync<SecurityException>();   
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                await CreateReturnToCopy();

                var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    A.Dummy<IUserContext>());

                Func<Task> action = async () => await handler.HandleAsync(new CopyReturn(@return.Id));

                await action.Should().ThrowAsync<SecurityException>();
            }
        }

        [Fact]
        public async Task HandleAsync_NoReturnFound_ArgumentExceptionExpected()
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var authorization = new AuthorizationBuilder().AllowEverything().Build();

                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    A.Dummy<IUserContext>());

                Func<Task> assertAction = async () => await handler.HandleAsync(new CopyReturn(Guid.NewGuid()));

                await assertAction.Should().ThrowAsync<ArgumentException>();
            }
        }

        [Fact]
        public async Task HandleAsync_ReturnCopied_CopiedReturnShouldBeAddedToContext()
        {
            void Action(Guid id)
            {   
                copiedReturn.Should().NotBeNull();
                copiedReturn.SubmittedById.Should().BeNull();
                copiedReturn.SubmittedBy.Should().BeNull();
                copiedReturn.CreatedBy.Should().BeNull();
                copiedReturn.CreatedById.ToLower().Should().Be(database.Model.AspNetUsers.First().Id.ToLower());
                copiedReturn.CreatedDate.Should().BeSameDateAs(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                copiedReturn.ReturnStatus.Should().Be(ReturnStatus.Created);
                copiedReturn.ParentId.Should().Be(@return.Id);
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_GivenReturnReportsOn_CopiedReturnShouldHaveSameValues()
        {
            void Action(Guid id)
            {
                copiedReturn.OperatorId.Should().Be(@return.OperatorId);

                var copiedReportsOn = database.WeeeContext.ReturnReportOns.Where(r => r.ReturnId == copiedReturn.Id).ToList();

                var originalReports = database.WeeeContext.ReturnReportOns.Where(r => r.ReturnId == @return.Id).ToList();

                copiedReportsOn.Count().Should().Be(originalReports.Count());
                copiedReportsOn.Select(r => r.Id).ToList().Should().NotContain(originalReports.Select(r => r.Id).ToList());
                copiedReportsOn.Select(r => r.ReportOnQuestionId).ToList().Should().Contain(originalReports.Select(r => r.ReportOnQuestionId).ToList());
            }

            await ActionAndAssert(Action);
        }

        [Fact]

        public async Task HandleAsync_ReturnCopied_CopiedOrganisationShouldBeTheSameAsOriginal()
        {
            void Action(Guid id)
            {
                copiedReturn.OperatorId.Should().Be(@return.OperatorId);
            }

            await ActionAndAssert(Action);
        }

        [Fact]
        public async Task HandleAsync_ReturnFound_OriginalReturnShouldBeInContext()
        {
            void Action(Guid id)
            {
                var originalReturn = database.WeeeContext.Returns.First(r => r.Id == @return.Id);

                originalReturn.Equals(@return).Should().BeTrue();
            }

            await ActionAndAssert(Action);
        }

        private async Task ActionAndAssert(Action<Guid> action)
        {
            using (database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var userContext = A.Fake<IUserContext>();

                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(database.Model.AspNetUsers.First().Id));

                await CreateReturnToCopy();
                
                var message = new CopyReturn(@return.Id);

                var authorization = new AuthorizationBuilder().AllowEverything().Build();
                
                handler = new CopyReturnHandler(authorization,
                    database.WeeeContext,
                    userContext);
                
                var result = await handler.HandleAsync(message);

                @return = database.WeeeContext.Returns.AsNoTracking().First(r => r.Id == message.ReturnId);
                copiedReturn = database.WeeeContext.Returns.First(r => r.Id == result);

                action(result);
            }
        }

        private async Task CreateReturnToCopy()
        {
            organisation = Organisation.CreateSoleTrader("Test Organisation");
            var @operator = new Operator(organisation);
            var quarter = new Quarter(2019, QuarterType.Q1);

            @return = new Domain.AatfReturn.Return(@operator, quarter, database.Model.AspNetUsers.First().Id) { ReturnStatus = ReturnStatus.Submitted };

            database.WeeeContext.Returns.Add(@return);

            await database.WeeeContext.SaveChangesAsync();

            var reportsOn = new List<ReturnReportOn>()
            {
                new ReturnReportOn(@return.Id, (int)ReportOnQuestionEnum.WeeeReceived)
            };

            database.WeeeContext.ReturnReportOns.AddRange(reportsOn);

            await database.WeeeContext.SaveChangesAsync();
        }
    }
}
