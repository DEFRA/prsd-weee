namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Organisation;
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
        private SchemeData schemeData1;
        private SchemeData schemeData2;

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

            A.CallTo(() => returnSchemeDataAccess.GetOrganisationByReturnId(returnId)).MustHaveHappened(Repeated.Exactly.Once);
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
            var organisation = A.Fake<EA.Weee.Domain.Organisation.Organisation>();

            A.CallTo(() => returnSchemeDataAccess.GetOrganisationByReturnId(A<Guid>._)).Returns(organisation);

            await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(organisation)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeListAndOperator_SchemeDataListShouldBeReturned()
        {
            var organisationData = A.Fake<Core.Organisations.OrganisationData>();
            var schemeList = A.Fake<SchemeData>();

            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._)).Returns(organisationData);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).Returns(schemeList);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._))
                .Returns(new List<ReturnScheme>() { A.Fake<ReturnScheme>() });
            var result = await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            result.OrganisationData.Should().Be(organisationData);
            result.SchemeDataItems.Should().Contain(schemeList);
            result.SchemeDataItems.Count().Should().Be(1);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeListAndOperator_SchemeDataListShouldBeOrderedBySchemeName()
        {
            var organisationData = A.Fake<Core.Organisations.OrganisationData>();

            Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
            Scheme scheme1 = new Scheme(organisation);
            Scheme scheme2 = new Scheme(organisation);

            var @return = A.Dummy<Return>();
            var returnScheme = new List<ReturnScheme>()
            {
                new ReturnScheme(scheme1, @return), 
                new ReturnScheme(scheme2, @return)
            };

            schemeData1 = A.Fake<SchemeData>();
            schemeData1.SchemeName = "Scheme D";
            schemeData1.SchemeStatus = Core.Shared.SchemeStatus.Approved;
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme1)).Returns(schemeData1);

            schemeData2 = A.Fake<SchemeData>();
            schemeData2.SchemeName = "Scheme A";
            schemeData2.SchemeStatus = Core.Shared.SchemeStatus.Approved;
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme2)).Returns(schemeData2);

            var schemeList = new List<SchemeData> {schemeData1, schemeData2 };

            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._)).Returns(organisationData);
            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(A<Guid>._)).Returns(returnScheme);
            var result = await handler.HandleAsync(A.Dummy<GetReturnScheme>());

            Assert.Collection(
             result.SchemeDataItems,
             (element1) => Assert.Equal("Scheme A", element1.SchemeName),
             (element2) => Assert.Equal("Scheme D", element2.SchemeName));
        }
    }
}