namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetAatfs
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.AatfReports;
    using Requests.Admin;
    using Requests.Admin.Aatf;
    using Xunit;
    using FacilityType = Core.AatfReturn.FacilityType;

    public class GetAatfSubmissionHistoryHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfSubmissionHistoryDataAccess dataAccess;
        private readonly IMapper mapper;
        private readonly GetAatfSubmissionHistoryHandler handler;
        private readonly Fixture fixture;

        public GetAatfSubmissionHistoryHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<IGetAatfSubmissionHistoryDataAccess>();
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            handler = new GetAatfSubmissionHistoryHandler(dataAccess, authorization, mapper);
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAatfSubmissionHistoryHandler(dataAccess, authorization, mapper);

            Func<Task> data = async () => await handler.HandleAsync(A.Dummy<GetAatfSubmissionHistory>());

            await Assert.ThrowsAsync<SecurityException>(data);
        }
    }
}
