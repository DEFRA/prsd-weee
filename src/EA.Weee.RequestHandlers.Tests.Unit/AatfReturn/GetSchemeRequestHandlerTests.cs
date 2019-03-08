namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Security;
    using Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class GetSchemeRequestHandlerTests
    {
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IReturnDataAccess returnDataAccess; 
        private readonly GetSchemeRequestHandler handler;
        private readonly IMapper mapper;
   
        public GetSchemeRequestHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMapper>();

            handler = new GetSchemeRequestHandler(weeeAuthorization, returnSchemeDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            var handlerLocal = new GetSchemeRequestHandler(authorization, returnSchemeDataAccess, mapper);

            Func<Task> action = async () => await handlerLocal.HandleAsync(A.Dummy<GetReturnScheme>());
            
            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetReturnSchemeRequest_SelectedSchemeShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();
            var request = new GetReturnScheme(returnId);

            await handler.HandleAsync(request);

            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenGetReturnSchemeRequest_OperatorShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();
            var request = new GetReturnScheme(returnId);

            await handler.HandleAsync(request);

            A.CallTo(() => returnSchemeDataAccess.GetOperatorByReturnId(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeList_SchemeListShouldBeMapped()
        {
            var schemeList = new List<ReturnScheme>() { A.Fake<ReturnScheme>(), A.Fake<ReturnScheme>() };
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._)).Returns(schemeList);

            await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            A.CallTo(() => mapper.Map<Scheme, SchemeData>(schemeList.ElementAt(0).Scheme)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(schemeList.ElementAt(1).Scheme)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenOperator_OperatorShouldBeMapped()
        {
            var @operator = A.Fake<Operator>();

            A.CallTo(() => returnSchemeDataAccess.GetOperatorByReturnId(A<Guid>._)).Returns(@operator);

            await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            A.CallTo(() => mapper.Map<Operator, OperatorData>(@operator)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeListAndOperator_SchemeDataListShouldBeReturned()
        {
            var @operator = A.Fake<OperatorData>();
            var schemeList = A.Fake<SchemeData>();

            A.CallTo(() => mapper.Map<Operator, OperatorData>(A<Operator>._)).Returns(@operator);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).Returns(schemeList);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._))
                .Returns(new List<ReturnScheme>() { A.Fake<ReturnScheme>() });
            var result = await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            result.OperatorData.Should().Be(@operator);
            result.SchemeDataItems.Should().Contain(schemeList);
            result.SchemeDataItems.Count().Should().Be(1);
        }
    }
}