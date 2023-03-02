namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FakeItEasy.Configuration;
    using RequestHandlers.AatfReturn.NonObligated;
    using Weee.Tests.Core;
    using Xunit;

    public class NonObligatedDataAccessTests
    {
        private const int CategoryCount = 14;
        private readonly Return aatfReturn = new ReturnWrapper(Guid.NewGuid());

        private readonly NonObligatedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        private Guid ReturnId => aatfReturn.Id;

        public NonObligatedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new NonObligatedDataAccess(context);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task InsertNonObligatedWeee(bool dcf)
        {
            // Arrange
            var nonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeee(aatfReturn, n, dcf, n * 15)).ToArray();
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet(new List<NonObligatedWeee>());
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);
            var callList = new List<IReturnValueArgumentValidationConfiguration<NonObligatedWeee>>();
            foreach (var item in nonObligatedWeees)
            {
                callList.Add(A.CallTo(() => dcfNonObligatedWeee.Add(item)));
            }
            var saveCall = A.CallTo(() => context.SaveChangesAsync());

            // Act
            await dataAccess.InsertNonObligatedWeee(ReturnId, nonObligatedWeees); // Placeholder

            // Assert
            foreach (var call in callList)
            {
                call.MustHaveHappenedOnceExactly();
            }
            saveCall.MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task InsertNonObligatedWeee_ExistingUpdated(bool dcf)
        {
            // Arrange
            var newNonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, n, dcf, n * 15)).ToArray();
            var existingNonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, n, dcf, n * 150)).ToArray();
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(existingNonObligatedWeees);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);
            var saveCall = A.CallTo(() => context.SaveChangesAsync());

            // Act
            await dataAccess.InsertNonObligatedWeee(ReturnId, newNonObligatedWeees);

            // Assert
            Assert.Equal(newNonObligatedWeees.Count(), existingNonObligatedWeees.Count());
            Assert.False(existingNonObligatedWeees
                .OrderBy(n => n.CategoryId)
                .Zip(newNonObligatedWeees.OrderBy(n => n.CategoryId), (e, n) => new { existing = e, expected = n })
                .Any(p => p.existing.CategoryId != p.expected.CategoryId
                    || p.existing.Dcf != p.expected.Dcf
                    || p.existing.Tonnage != p.expected.Tonnage), "Non-obligated WEEE collection does not match expected.");
            saveCall.MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateNonObligatedWeeeAmounts_ExistingAmountsUpdated(bool dcf)
        {
            // Arrange
            var existingNonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, n, dcf, n * 15)).ToArray();
            var updateArg = existingNonObligatedWeees.Select(n => new Tuple<Guid, decimal?>(n.Id, n.Tonnage * 10)).ToArray();
            var saveCall = A.CallTo(() => context.SaveChangesAsync());
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(existingNonObligatedWeees);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);

            // Act
            await dataAccess.UpdateNonObligatedWeeeAmounts(ReturnId, updateArg);

            // Assert
            saveCall.MustHaveHappenedOnceExactly();
            Assert.Equal(updateArg.Count(), existingNonObligatedWeees.Where(n => updateArg.First(a => a.Item1 == n.Id).Item2 == n.Tonnage).Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateNonObligatedWeeeAmounts_DuplicatesRemoved(bool dcf)
        {
            // Arrange
            const int amountUpdateMultiplier = 5;
            var existingNonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, n, dcf, n * 15)).ToArray();
            var duplicateExistingNonObligatedWeees = Enumerable.Range(1, CategoryCount).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, n, dcf, n * 100)).ToArray();
            var updateArg = existingNonObligatedWeees.Select(n => new Tuple<Guid, decimal?>(n.Id, n.Tonnage * amountUpdateMultiplier)).ToArray();
            var repoNonObligatedWeee = existingNonObligatedWeees.Concat(duplicateExistingNonObligatedWeees).ToArray();
            var saveCall = A.CallTo(() => context.SaveChangesAsync());
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(repoNonObligatedWeee);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);
            var removeCall = A.CallTo(() => dcfNonObligatedWeee.RemoveRange(A<IEnumerable<NonObligatedWeee>>.Ignored));
            IEnumerable<NonObligatedWeee> removeElements = null;
            removeCall.Invokes(c => removeElements = c.Arguments.First() as IEnumerable<NonObligatedWeee>);

            // Act
            await dataAccess.UpdateNonObligatedWeeeAmounts(ReturnId, updateArg);

            // Assert
            saveCall.MustHaveHappenedOnceExactly();
            // Assert that the removed elements are correct
            Assert.NotNull(removeElements);
            Assert.Equal(duplicateExistingNonObligatedWeees.Count(), removeElements.Count());
            Assert.False(duplicateExistingNonObligatedWeees.Any(n => !removeElements.Any(r => r.Id == n.Id)), "Removed list does not contain all expected items.");
            // Assert that the updated elements are correct
            var matched = existingNonObligatedWeees.Select(n => new { initial = n, repo = dcfNonObligatedWeee.FirstOrDefault(r => r.Id == n.Id) });
            Assert.False(matched.Any(m => m.repo == null), "Not all updated elements are in the actual repository collection.");
            Assert.False(matched.Any(m => (m.initial.Tonnage * amountUpdateMultiplier) == m.repo.Tonnage), "Not all amounts are correct.");
        }

        [Fact]
        public async Task FetchNonObligatedWeeeForReturnWithoutDuplicates_NoDuplicates()
        {
            // Arrange
            var existingNonObligatedWeees = Enumerable.Range(0, CategoryCount * 2).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, 1 + (n % CategoryCount), n > 13, n * 15));
            var saveCall = A.CallTo(() => context.SaveChangesAsync());
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(existingNonObligatedWeees);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);

            // Act
            var response = await dataAccess.FetchNonObligatedWeeeForReturn(aatfReturn.Id);

            // Assert
            Assert.Equal(existingNonObligatedWeees.Count(), response.Count());
        }

        [Fact]
        public async Task FetchNonObligatedWeeeForReturnWithoutDuplicates_WithDuplicates()
        {
            // Arrange
            var existingNonObligatedWeees = Enumerable.Range(0, CategoryCount * 2).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, 1 + (n % CategoryCount), n > (CategoryCount - 1), (1 + n) * 15));
            var existingDuplicateNonObligatedWeees = Enumerable.Range(0, CategoryCount * 2).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, 1 + (n % CategoryCount), n > (CategoryCount - 1), (1 + n) * 15));
            var repoWeee = existingNonObligatedWeees.Concat(existingDuplicateNonObligatedWeees);
            // Check to prove the initial data is correct, 
            Assert.Equal(CategoryCount * 4, repoWeee.Count());
            var saveCall = A.CallTo(() => context.SaveChangesAsync());
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(repoWeee);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);

            // Act
            var response = await dataAccess.FetchNonObligatedWeeeForReturn(aatfReturn.Id);

            // Assert
            Assert.Equal(existingNonObligatedWeees.Count(), response.Count());
            var responseDcfWeee = response.Where(n => n.Dcf == true);
            var responseNotDcfWeee = response.Where(n => n.Dcf == false);
            Assert.Equal(CategoryCount, responseDcfWeee.Count());
            Assert.Equal(CategoryCount, responseNotDcfWeee.Count());
            Assert.DoesNotContain(responseNotDcfWeee, n => n.Tonnage != n.CategoryId * 15);
            Assert.DoesNotContain(responseDcfWeee, n => n.Tonnage != (n.CategoryId + 14) * 15);
        }

        [Fact]
        public async Task FetchNonObligatedWeeeForReturnWithoutDuplicates_FetchesOnlySpecifiedReturnWeee()
        {
            // Arrange
            int numberWeeeToCreate = CategoryCount * 2;
            // Arrange Return to fetch
            var existingNonObligatedWeees = Enumerable.Range(0, numberWeeeToCreate).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), aatfReturn, 1 + (n % CategoryCount), n > (CategoryCount - 1), (1 + n) * 15));

            // Arrange another Return data
            var otherAatfReturn = new ReturnWrapper(Guid.NewGuid());
            var existingNonObligatedWeeesOtherReturn = Enumerable.Range(0, numberWeeeToCreate).Select(n => new NonObligatedWeeeWrapper(Guid.NewGuid(), otherAatfReturn, 1 + (n % CategoryCount), n > (CategoryCount - 1), (1 + n) * 200));

            // Build the Repo collection
            var repoWeee = existingNonObligatedWeees.Concat(existingNonObligatedWeeesOtherReturn);
            var saveCall = A.CallTo(() => context.SaveChangesAsync());
            var dcfNonObligatedWeee = dbContextHelper.GetAsyncEnabledDbSet<NonObligatedWeee>(repoWeee);
            A.CallTo(() => context.NonObligatedWeee).Returns(dcfNonObligatedWeee);

            // Act
            var response = await dataAccess.FetchAllNonObligateWeeeForReturn(aatfReturn.Id);

            // Assert
            saveCall.MustNotHaveHappened();
            Assert.Equal(numberWeeeToCreate, response.Count());
            Assert.DoesNotContain(response, n => n.ReturnId != aatfReturn.Id);
            Assert.DoesNotContain(response, n => n.ReturnId == otherAatfReturn.Id);
        }

        public class NonObligatedWeeeWrapper : NonObligatedWeee
        {
            public NonObligatedWeeeWrapper(Guid entityId, Return aatfReturn, int categoryId, bool dcf, decimal? tonnage)
                : base(aatfReturn, categoryId, dcf, tonnage)
            {
                EntityId = entityId;
                ReturnId = aatfReturn.Id;
            }

            public Guid EntityId { get; private set; }

            public override Guid Id
            {
                get
                {
                    return EntityId;
                }
            }
        }

        public class ReturnWrapper : Return
        {
            public ReturnWrapper(Guid entityId)
            {
                EntityId = entityId;
            }

            public Guid EntityId { get; private set; }

            public override Guid Id
            {
                get
                {
                    return EntityId;
                }
            }
        }
    }
}
