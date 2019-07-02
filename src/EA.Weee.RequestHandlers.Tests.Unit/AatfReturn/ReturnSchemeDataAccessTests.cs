namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
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
        public async Task GetSelectedSchemesByReturnId_ReturnSchemeNamesInAscendingOrder()
        {
            var returnId = Guid.NewGuid();
            var returnScheme1 = A.Fake<ReturnScheme>();
            var returnScheme2 = A.Fake<ReturnScheme>();

            A.CallTo(() => returnScheme1.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnScheme2.Scheme).Returns(A.Fake<Scheme>());
            A.CallTo(() => returnScheme1.ReturnId).Returns(returnId);
            A.CallTo(() => returnScheme2.ReturnId).Returns(returnId);
            A.CallTo(() => returnScheme1.Scheme.SchemeName).Returns("Scheme Q");
            A.CallTo(() => returnScheme2.Scheme.SchemeName).Returns("Scheme D");
            A.CallTo(() => context.ReturnScheme).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ReturnScheme>() { returnScheme1, returnScheme2 }));

            var result = await dataAccess.GetSelectedSchemesByReturnId(returnId);

            Assert.Collection(
           result,
           (element1) => Assert.Equal("Scheme D", element1.Scheme.SchemeName),
           (element2) => Assert.Equal("Scheme Q", element2.Scheme.SchemeName));
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
    }
}
