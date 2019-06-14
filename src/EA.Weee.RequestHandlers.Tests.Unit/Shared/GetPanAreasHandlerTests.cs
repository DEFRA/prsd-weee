namespace EA.Weee.RequestHandlers.Tests.Unit.Shared
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetPanAreasHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private readonly IMap<PanArea, PanAreaData> mapper;
        private GetPanAreasHandler handler;

        public GetPanAreasHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            authorization = A.Fake<IWeeeAuthorization>();
            mapper = A.Fake<IMap<PanArea, PanAreaData>>();

            A.CallTo(() => context.PanAreas).Returns(dbHelper.GetAsyncEnabledDbSet(new List<PanArea>
            {
                new PanArea() { Name = "North", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new PanArea() { Name = "East", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new PanArea() { Name = "South", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
                new PanArea() { Name = "West", CompetentAuthorityId = Guid.NewGuid(), Id = Guid.NewGuid() },
            }));

            handler = new GetPanAreasHandler(context, authorization, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoInternallAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetPanAreasHandler(A.Fake<WeeeContext>(), authorization, mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetPanAreas>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_ReturnsAllPanAreas()
        {
            var message = new GetPanAreas();

            var result = await handler.HandleAsync(message);

            result.Count.Should().Be(4);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_LocalAreasShouldBeOrderedByName()
        {
            var message = new GetPanAreas();

            var result = await handler.HandleAsync(message);

            result.Should().BeInAscendingOrder(x => x.Name);
        }
        /*
        [Fact]
        public async Task HandleAsync_GivenMessage_ResultShouldBeListOfPanAreaData()
        {
            var message = new GetPanAreas();

            var result = await handler.HandleAsync(message);

            foreach (var panarea in result)
            {
                panarea.Should().BeOfType(typeof(PanAreaData));
            }
        }
        */
        [Fact]
        public async Task HandleAsync_GivenMessage_PanAreasShouldBeMapped()
        {
            var message = new GetPanAreas();

            var result = await handler.HandleAsync(message);

            foreach (var panarea in context.PanAreas)
            {
                foreach (var mappedpanarea in result)
                {
                    if (mappedpanarea.Name == panarea.Name)
                    {
                        mappedpanarea.Id.Should().Be(panarea.Id);
                        mappedpanarea.CompetentAuthorityId.Should().Be(panarea.CompetentAuthorityId);
                    }
                }
            }
        }
    }
}
