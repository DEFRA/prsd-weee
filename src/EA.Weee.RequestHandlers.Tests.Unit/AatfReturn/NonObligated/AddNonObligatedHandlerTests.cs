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
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(new List<NonObligatedWeee>());

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedFullSet()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);
            
            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustHaveHappened();

            submittedCollection.Count().Should().Be(message.CategoryValues.Count());
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_PartialSet_PreExistingNonConflicting()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = CreateRepoNonObligatedHalfSet(false);
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedHalfSet()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustHaveHappened();
            submittedCollection.Count().Should().Be(message.CategoryValues.Count());
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_PartialSet_PreExistingConflicting()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = CreateRepoNonObligatedFullSet();
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedHalfSet()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_PreExistingConflicting()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = CreateRepoNonObligatedFullSet();
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedFullSet()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_Duplicating()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = new List<NonObligatedWeee>();
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedFullSet().Concat(CreateMessageNonObligatedFullSet()).ToList()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustHaveHappened();

            //submittedCollection.Count().Should().NotBe(message.CategoryValues.Count());
            submittedCollection.Count().Should().Be(message.CategoryValues.Count() / 2);
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_DuplicatingAndConflicting()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = CreateRepoNonObligatedFullSet();
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedFullSet().Concat(CreateMessageNonObligatedFullSet()).ToList()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustNotHaveHappened();
            submittedCollection.Should().BeNull();
        }

        [Fact]
        public async void HandleAsync_AddNonObligate_FullSet_PartialConflicting()
        {
            // Arrange
            var getReturnById = A.CallTo(() => this.returnDataAccess.GetById(this.returnId));
            getReturnById.Returns(@return);

            var existingRecords = CreateRepoNonObligatedHalfSet(false);
            var fetchNonObligated = A.CallTo(() => this.nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(this.returnId));
            fetchNonObligated.Returns(existingRecords);

            IEnumerable<NonObligatedWeee> submittedCollection = null;

            var sumbitCall = A.CallTo(() => this.nonObligatedDataAccess.Submit(A<IEnumerable<NonObligatedWeee>>.Ignored));
            sumbitCall.Invokes(a => submittedCollection = a.GetArgument<IEnumerable<NonObligatedWeee>>("nonObligated"));

            var message = new AddNonObligated()
            {
                Dcf = false,
                OrganisationId = this.organisationId,
                ReturnId = this.returnId,
                CategoryValues = CreateMessageNonObligatedFullSet()
            };

            // Act
            bool result = await this.handler.HandleAsync(message);

            // Assert
            getReturnById.MustHaveHappened();
            fetchNonObligated.MustHaveHappened();
            sumbitCall.MustHaveHappened();
            submittedCollection.Count().Should().Be(message.CategoryValues.Count() - existingRecords.Count());
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
