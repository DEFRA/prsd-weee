namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetSchemes;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemesByOrganisationIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private GetSchemesByOrganisationIdHandler handler;

        public GetSchemesByOrganisationIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IGetSchemesDataAccess>();
            this.schemeMap = A.Fake<IMap<Scheme, SchemeData>>();

            handler = new GetSchemesByOrganisationIdHandler(authorization, schemeMap, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetSchemesByOrganisationIdHandler(authorization, schemeMap, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSchemesByOrganisationId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_GetSchemesShouldBeCalled()
        {
            var result = await handler.HandleAsync(A.Dummy<GetSchemesByOrganisationId>());

            A.CallTo(() => dataAccess.GetSchemes()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ListOfSchemesShouldBeReturned()
        {
            var schemeList = A.CollectionOfFake<Scheme>(2).ToList();
            var schemeDataList = new List<SchemeData>();

            foreach (var scheme in schemeList)
            {
                var data = schemeMap.Map(scheme);
                schemeDataList.Add(data);
            }

            A.CallTo(() => dataAccess.GetSchemes()).Returns(schemeList);

            var result = await handler.HandleAsync(A.Dummy<GetSchemesByOrganisationId>());

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(schemeDataList);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_OnlyNotPendingSchemesShouldBeReturned()
        {
            var notPendingScheme = A.Fake<Scheme>();
            var pendingScheme = A.Fake<Scheme>();
            A.CallTo(() => pendingScheme.SchemeStatus).Returns(SchemeStatus.Pending);

            var schemeList = new List<Scheme>() { notPendingScheme, pendingScheme};

            A.CallTo(() => dataAccess.GetSchemes()).Returns(schemeList);
            A.CallTo(() => schemeMap.Map(pendingScheme)).Returns(new SchemeData());
            A.CallTo(() => schemeMap.Map(notPendingScheme)).Returns(new SchemeData() { SchemeStatus = Core.Shared.SchemeStatus.Approved });

            var result = await handler.HandleAsync(A.Dummy<GetSchemesByOrganisationId>());

            result.Should().HaveCount(1);
            result.Count(s => s.SchemeStatus == Core.Shared.SchemeStatus.Pending).Should().Be(0);
        }
    }
}
