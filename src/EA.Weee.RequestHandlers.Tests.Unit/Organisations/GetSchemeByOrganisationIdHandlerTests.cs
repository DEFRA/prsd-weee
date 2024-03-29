﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemeByOrganisationIdHandlerTests
    {
        private readonly GetSchemeByOrganisationIdHandler handler;
        private readonly IWeeeAuthorization authorization;
        private readonly ISchemeDataAccess dataAccess;
        private readonly IMapper mapper;

        public GetSchemeByOrganisationIdHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            dataAccess = A.Fake<ISchemeDataAccess>();
            mapper = A.Fake<IMapper>();

            handler = new GetSchemeByOrganisationIdHandler(dataAccess, mapper, authorization);
        }

        [Fact]
        public async Task HandleAsync_GivenNotOrganisationUser_ThrowsSecurityException()
        {
            var localAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var localHandler = new GetSchemeByOrganisationIdHandler(dataAccess, mapper, localAuthorization);

            Func<Task<SchemeData>> action = async () => await localHandler.HandleAsync(A.Dummy<GetSchemeByOrganisationId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationId_SchemeShouldBeRetrieved()
        {
            var request = new GetSchemeByOrganisationId(Guid.NewGuid());

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeNotFound_ArgumentExceptionExpected()
        {
            var request = new GetSchemeByOrganisationId(Guid.NewGuid());

            A.CallTo(() => dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).Returns((Scheme)null);

            SchemeData result = await handler.HandleAsync(request);

            Assert.Null(result);

            A.CallTo(() => mapper.Map<Scheme, SchemeData>(A<Scheme>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeFound_SchemeDataShouldBeMapped()
        {
            var scheme = A.Fake<Scheme>();
            var request = new GetSchemeByOrganisationId(Guid.NewGuid());

            A.CallTo(() => dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).Returns(scheme);

            var result = await handler.HandleAsync(request);

            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeFound_SchemeDataShouldBeReturned()
        {
            var scheme = A.Fake<Scheme>();
            var request = new GetSchemeByOrganisationId(Guid.NewGuid());
            var schemeData = new SchemeData();

            A.CallTo(() => dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).Returns(scheme);
            A.CallTo(() => mapper.Map<Scheme, SchemeData>(scheme)).Returns(schemeData);

            var result = await handler.HandleAsync(request);

            result.Should().Be(schemeData);
        }
    }
}
