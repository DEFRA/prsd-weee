namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using EA.Weee.Core.Admin;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
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
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private GetLocalAreasHandler handler;

        public GetLocalAreasHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            authorization = A.Fake<IWeeeAuthorization>();

            A.CallTo(() => context.LocalAreas).Returns(dbHelper.GetAsyncEnabledDbSet(new List<LocalArea>
            {
                new LocalArea() { Name = "North", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new LocalArea() { Name = "East", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new LocalArea() { Name = "South", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new LocalArea() { Name = "West", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
            }));

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

        [Fact]
        public async Task HandleAsync_GivenMessage_ReturnsAllLocalAreas()
        {
            var message = new GetLocalAreas();

            var result = await handler.HandleAsync(message);

            result.Count.Should().Be(4);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_LocalAreasShouldBeOrderedByName()
        {
            var message = new GetLocalAreas();

            var result = await handler.HandleAsync(message);

            result.Should().BeInAscendingOrder(x => x.Name);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_ResultShouldBeListOfLocalAreaData()
        {
            var message = new GetLocalAreas();

            var result = await handler.HandleAsync(message);

            result.Should().AllBeOfType(typeof(LocalAreaData));
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_LocalAreasShouldBeMapped()
        {
            var message = new GetLocalAreas();

            var result = await handler.HandleAsync(message);

            foreach (var localarea in context.LocalAreas)
            {
                foreach (var mappedlocalarea in result)
                {
                    if (mappedlocalarea.Name == localarea.Name)
                    {
                        mappedlocalarea.Id.Should().Be(localarea.Id);
                        mappedlocalarea.CompetentAuthorityId.Should().Be(localarea.CompetentAuthorityId);
                    }
                }
            }
        }
    }
}
