namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnSchemeDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly ReturnSchemeDataAccess dataAccess;
        private readonly DbContextHelper dbHelper;

        public ReturnSchemeDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbHelper = new DbContextHelper();

            dataAccess = new ReturnSchemeDataAccess(context);
        }

        [Fact]
        public async Task Submit_GivenSchemeAddedToContext_ChangesShouldBeSaved()
        {
            var items = A.Fake<List<ReturnScheme>>();

            await dataAccess.Submit(items);

            A.CallTo(() => context.ReturnScheme.AddRange(items)).MustHaveHappened().Then(
                A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Fact]
        public async Task GetSelectedSchemesByReturnId_GivenMatchingReturnId_ReturnSchemesShouldBeReturned()
        {
            var returnId = Guid.NewGuid();
            var returnSchemeMatch = A.Fake<ReturnScheme>();
            var returnSchemeNoMatch = A.Fake<ReturnScheme>();

            A.CallTo(() => returnSchemeMatch.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnSchemeNoMatch.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnSchemeMatch.ReturnId).Returns(returnId);
            A.CallTo(() => context.ReturnScheme).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ReturnScheme>() { returnSchemeNoMatch, returnSchemeMatch }));

            var result = await dataAccess.GetSelectedSchemesByReturnId(returnId);

            result.Should().BeEquivalentTo(returnSchemeMatch);
        }

        [Fact]
        public async Task GetOperatorByReturnId_GivenReturnIdExists_OperatorForReturnShouldBeReturned()
        {
            var returnId = Guid.NewGuid();
            var returnMatch = A.Fake<Return>();

            A.CallTo(() => returnMatch.Id).Returns(returnId);
            A.CallTo(() => context.Returns).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Return>() { returnMatch }));

            var result = await dataAccess.GetOrganisationByReturnId(returnId);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetOperatorByReturnId_GivenReturnIdDoesNotExist_NullShouldBeReturned()
        {
            var returnId = Guid.NewGuid();
            var returnMatch = A.Fake<Return>();
            
            A.CallTo(() => context.Returns).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Return>() { returnMatch }));

            var result = await dataAccess.GetOrganisationByReturnId(returnId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveReturnScheme_GivenSchemeIdReturnId_PCSforReturnShouldBeReturned()
        {
            var returnId = Guid.NewGuid();
            var returnIdNoMatch = Guid.NewGuid();
            var schemeId1 = Guid.NewGuid();
            var schemeIds = new List<Guid>();
            schemeIds.Add(schemeId1);

            var returnSchemeMatch = A.Fake<ReturnScheme>();
            var returnSchemeNoMatch = A.Fake<ReturnScheme>();

            A.CallTo(() => returnSchemeMatch.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnSchemeMatch.SchemeId).Returns(schemeId1);
            A.CallTo(() => returnSchemeMatch.ReturnId).Returns(returnId);

            A.CallTo(() => returnSchemeNoMatch.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnSchemeNoMatch.SchemeId).Returns(schemeId1);
            A.CallTo(() => returnSchemeNoMatch.ReturnId).Returns(returnIdNoMatch);

            List<WeeeReceived> weeeReceived = new List<WeeeReceived>();

            WeeeReceived weee = new WeeeReceived(schemeId1, A.Dummy<Guid>(), returnId);           
            WeeeReceived weee1 = new WeeeReceived(schemeId1, A.Dummy<Guid>(), returnIdNoMatch);
            weeeReceived.Add(weee);
            weeeReceived.Add(weee1);

            var list = new List<WeeeReceivedAmount>();
            var weeeReceivedAmount = A.Fake<WeeeReceivedAmount>();

            A.CallTo(() => weeeReceivedAmount.Id).Returns(A.Dummy<Guid>());
            list.Add(weeeReceivedAmount);
            A.CallTo(() => context.WeeeReceivedAmount).Returns(dbHelper.GetAsyncEnabledDbSet(list));

            A.CallTo(() => context.WeeeReceived).Returns(dbHelper.GetAsyncEnabledDbSet(weeeReceived));
            A.CallTo(() => context.ReturnScheme).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ReturnScheme>() { returnSchemeNoMatch, returnSchemeMatch }));

            await dataAccess.RemoveReturnScheme(schemeIds, returnId);

            A.CallTo(() => context.WeeeReceived.Remove(weee)).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
            A.CallTo(() => context.ReturnScheme.Remove(returnSchemeMatch)).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
