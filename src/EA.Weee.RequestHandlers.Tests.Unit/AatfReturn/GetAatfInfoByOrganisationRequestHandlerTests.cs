namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.Specification;
    using Requests.AatfReturn;
    using Xunit;

    public class GetAatfInfoByOrganisationRequestHandlerTests
    {
        private readonly GetAatfInfoByOrganisationRequestHandler handler;
        private readonly IMap<Aatf, AatfData> mapper;
        private readonly IGenericDataAccess dataAccess;

        public GetAatfInfoByOrganisationRequestHandlerTests()
        {
            mapper = A.Fake<IMap<Aatf, AatfData>>();
            dataAccess = A.Fake<IGenericDataAccess>();

            handler = new GetAatfInfoByOrganisationRequestHandler(mapper, dataAccess);
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var id = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfByOrganisation(id));

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationSpecification>.That.Matches(c => c.OrganisationId == id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenAatfData_AatfDataShouldBeMapped()
        {
            var aatfs = Aatfs();

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationSpecification>._)).Returns(aatfs);

            await handler.HandleAsync(A.Dummy<GetAatfByOrganisation>());

            for (var i = 0; i < aatfs.Count; i++)
            {
                A.CallTo(() => mapper.Map(aatfs.ElementAt(i))).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async void HandleAsync_GivenMappedAatfData_AatfDataShouldBeReturn()
        {
            var aatfDatas = new List<AatfData>()
            {
                A.Fake<AatfData>(),
                A.Fake<AatfData>()
            }.ToArray();

            A.CallTo(() => dataAccess.GetManyByExpression(A<AatfsByOrganisationSpecification>._)).Returns(Aatfs());
            A.CallTo(() => mapper.Map(A<Aatf>._)).ReturnsNextFromSequence(aatfDatas);

            var result = await handler.HandleAsync(A.Dummy<GetAatfByOrganisation>());

            foreach (var aatfData in aatfDatas)
            {
                result.Should().Contain(aatfData);
            }

            result.Count().Should().Be(aatfDatas.Length);
        }

        private List<Aatf> Aatfs()
        {
            var aatfs = new List<Aatf>()
            {
                A.Fake<Aatf>(),
                A.Fake<Aatf>()
            };
            return aatfs;
        }
    }
}
