namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class AddReturnUploadHandlerTests
    {
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IGenericDataAccess operDataAccess;
        private AddReturnHandler handler;

        public AddReturnUploadHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            operDataAccess = A.Fake<IGenericDataAccess>();

            handler = new AddReturnHandler(weeeAuthorization, returnDataAccess, organisationDataAccess, operDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IGenericDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoOrganisationAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new AddReturnHandler(authorization,
                A.Dummy<IReturnDataAccess>(),
                A.Dummy<IOrganisationDataAccess>(),
                A.Dummy<IGenericDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_DataAccessSubmitsIsCalled()
        {
            const int year = 2019;
            const int quarter = 1;

            var request = new AddReturn { OrganisationId = Guid.NewGuid(), Quarter = quarter,  Year = year };
        
            var @return = A.Dummy<Return>();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => organisationDataAccess.GetById(request.OrganisationId)).Returns(organisation);

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.Submit(A<Return>.That.Matches(c => c.Quarter.Year == year && (int)c.Quarter.Q == quarter))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_OperatorShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();
            var organisation = A.Fake<Organisation>();
            var request = new AddReturn { OrganisationId = organisationId };

            Expression<Func<Operator, bool>> exp = c => c.Organisation.Id == request.OrganisationId;

            A.CallTo(() => organisationDataAccess.GetById(A<Guid>._)).Returns(organisation);
            A.CallTo(() => organisation.Id).Returns(organisationId);

            await handler.HandleAsync(request);

            var expressionComparar = new ExpressionEqualityComparer();
            A.CallTo(() => operDataAccess.GetById<Operator>(A<Expression<Func<Operator, bool>>>.That.IsSameAs(exp))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnRequest_OrganisationShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();

            var request = new AddReturn { OrganisationId = organisationId };

            await handler.HandleAsync(request);

            A.CallTo(() => organisationDataAccess.GetById(organisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
