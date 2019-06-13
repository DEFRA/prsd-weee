namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin;
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

    public class GetLocalAreasHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private GetLocalAreasHandler handler;

        public GetLocalAreasHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new GetLocalAreasHandler(context, authorization);
        }

        [Fact]
        public async Task HandleAsync_NoInternallAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetLocalAreasHandler(A.Fake<WeeeContext>(), authorization);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetLocalAreas>());

            await action.Should().ThrowAsync<SecurityException>();
        }
    }
}
