namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using FakeItEasy;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.NonObligated;
    using Xunit;

    public class AddNonObligatedHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly AddNonObligatedHandler handler;

        private readonly Guid returnId = Guid.NewGuid();
        private readonly Guid organisationId = Guid.NewGuid();
        private readonly Return @return;

        private AddNonObligated message;
        private List<NonObligatedWeee> existingRecords;
        private IEnumerable<NonObligatedWeee> submittedCollection;
        private IReturnValueConfiguration<System.Threading.Tasks.Task<EA.Weee.Domain.AatfReturn.Return>> getReturnById;
        private IReturnValueArgumentValidationConfiguration<System.Threading.Tasks.Task<System.Collections.Generic.List<EA.Weee.Domain.AatfReturn.NonObligatedWeee>>> fetchNonObligated;
        private IReturnValueArgumentValidationConfiguration<System.Threading.Tasks.Task> sumbitCall;

        public AddNonObligatedHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.nonObligatedDataAccess = A.Fake<INonObligatedDataAccess>();
            this.returnDataAccess = A.Fake<IReturnDataAccess>();
            this.handler = new AddNonObligatedHandler(authorization, nonObligatedDataAccess, returnDataAccess);
            var org = new Organisation();
            this.@return = new Return(new Organisation(), new Quarter(2019, QuarterType.Q4), "someone", FacilityType.Aatf);
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet()
        {
            existingRecords = new List<NonObligatedWeee>();
            message = CreateMessage(CreateMessageNonObligatedFullSet());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustHaveHappened();
            submittedCollection.Count().Should().Be(message.CategoryValues.Count());
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_PartialSet_PreExistingNonConflicting()
        {
            existingRecords = CreateRepoNonObligatedHalfSet(false);
            message = CreateMessage(CreateMessageNonObligatedHalfSet());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustHaveHappened();
            this.submittedCollection.Count().Should().Be(message.CategoryValues.Count());
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_PartialSet_PreExistingConflicting()
        {
            existingRecords = CreateRepoNonObligatedFullSet();
            message = CreateMessage(CreateMessageNonObligatedHalfSet());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_PreExistingConflicting()
        {
            existingRecords = CreateRepoNonObligatedFullSet();
            message = CreateMessage(CreateMessageNonObligatedFullSet());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_Duplicating()
        {
            existingRecords = new List<NonObligatedWeee>();
            message = CreateMessage(CreateMessageNonObligatedFullSet().Concat(CreateMessageNonObligatedFullSet()).ToList());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustHaveHappened();
            submittedCollection.Count().Should().Be(message.CategoryValues.Count() / 2);
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_DuplicatingAndConflicting()
        {
            existingRecords = CreateRepoNonObligatedFullSet();
            message = CreateMessage(CreateMessageNonObligatedFullSet().Concat(CreateMessageNonObligatedFullSet()).ToList());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_PartialConflicting()
        {
            existingRecords = CreateRepoNonObligatedHalfSet(false);
            message = CreateMessage(CreateMessageNonObligatedFullSet());

            // Arrange
            Arrange();

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            AssertMandatoryCall();
            sumbitCall.MustHaveHappened();
            submittedCollection.Count().Should().Be(message.CategoryValues.Count() - existingRecords.Count());
        }

        private AddNonObligated CreateMessage(IList<NonObligatedValue> values)
        {
            return new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = values
            };
        }

        private void Arrange()
        {
            this.getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            this.getReturnById.Returns(@return);

            this.fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            this.fetchNonObligated.Returns(existingRecords);

            this.sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            this.sumbitCall.Invokes(a => this.submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));
        }

        private void AssertMandatoryCall()
        {
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
        }

        private List<NonObligatedValue> CreateMessageNonObligatedFullSet()
        {
            return CreateMessageNonObligatedHalfSet().Concat(CreateMessageNonObligatedHalfSet(false)).ToList();
        }

        private List<NonObligatedValue> CreateMessageNonObligatedHalfSet(bool isFirstHalf = true)
        {
            return Enumerable.Range(isFirstHalf ? 1 : 8, 7).Select(n => new NonObligatedValue(n, CreateTonnageForCategory(n), false, Guid.NewGuid())).ToList();
        }
        private int CreateTonnageForCategory(int category)
        {
            return category * 25;
        }

        private List<NonObligatedWeee> CreateRepoNonObligatedFullSet()
        {
            return CreateRepoNonObligatedHalfSet().Concat(CreateRepoNonObligatedHalfSet(false)).ToList();
        }

        private List<NonObligatedWeee> CreateRepoNonObligatedHalfSet(bool isFirstHalf = true)
        {
            return Enumerable.Range(isFirstHalf ? 1 : 8, 7).Select(n => new NonObligatedWeee(this.@return, n, false, CreateTonnageForCategory(n))).ToList();
        }
    }
}
