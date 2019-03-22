﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSentOnAatfSiteHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess sentOnDataAccess;
        private readonly GetSentOnAatfSiteHandler handler;
        private readonly IMap<AatfAddress, AatfAddressData> mapper;

        public GetSentOnAatfSiteHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            sentOnDataAccess = A.Fake<ISentOnAatfSiteDataAccess>();
            mapper = A.Fake<IMap<AatfAddress, AatfAddressData>>();

            handler = new GetSentOnAatfSiteHandler(authorization, sentOnDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetSentOnAatfSiteHandler(authorization, sentOnDataAccess, mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var id = Guid.NewGuid();

            await handler.HandleAsync(new GetSentOnAatfSite(id));

            A.CallTo(() => sentOnDataAccess.GetWeeeSentOnAddress(id)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
