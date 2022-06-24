namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain;
    using Mappings;
    using Prsd.Core;
    using RequestHandlers.Aatf;
    using Xunit;

    public class GetAatfInfoByOrganisationRequestHandlerTests
    {
        private GetAatfInfoByOrganisationRequestHandler handler;
        private readonly IMap<AatfWithSystemDateMapperObject, AatfData> mapper;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetAatfInfoByOrganisationRequestHandlerTests()
        {
            mapper = A.Fake<IMap<AatfWithSystemDateMapperObject, AatfData>>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            handler = new GetAatfInfoByOrganisationRequestHandler(mapper, aatfDataAccess, authorization, systemDataDataAccess);
        }

        [Fact]
        public async void HandleAsync_NoOrganisationOrInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalOrOrganisationAccess().Build();

            handler = new GetAatfInfoByOrganisationRequestHandler(A.Fake<IMap<AatfWithSystemDateMapperObject, AatfData>>(), A.Fake<IAatfDataAccess>(), authorization, systemDataDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfByOrganisation>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessShouldBeCalled()
        {
            var id = Guid.NewGuid();

            await handler.HandleAsync(new GetAatfByOrganisation(id));

            A.CallTo(() => aatfDataAccess.GetAatfsForOrganisation(id)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void HandleAsync_GivenRequest_SystemSettingsShouldBeRetrieved()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            await handler.HandleAsync(new GetAatfByOrganisation(id));

            //assert
            A.CallTo(() => systemDataDataAccess.Get()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenAatfDataWithUseFixedDate_AatfDataShouldBeMapped()
        {
            var aatfs = Aatfs();
            var systemData = A.Fake<SystemData>();
            var date = new DateTime(2019, 1, 1);

            systemData.ToggleFixedCurrentDateUsage(true);
            systemData.UpdateFixedCurrentDate(date);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(systemData);
            A.CallTo(() => aatfDataAccess.GetAatfsForOrganisation(A<Guid>._)).Returns(aatfs);

            await handler.HandleAsync(A.Dummy<GetAatfByOrganisation>());

            foreach (var aatfData in aatfs)
            {
                A.CallTo(() => mapper.Map(A<AatfWithSystemDateMapperObject>.That.Matches(a =>
                    Equals(a.Aatf, aatfData) && a.SystemDateTime == date))).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async void HandleAsync_GivenAatfDataWithUseCurrentDate_AatfDataShouldBeMapped()
        {
            var date = new DateTime(2020, 1, 1);
            SystemTime.Freeze(date);
            var aatfs = Aatfs();
            var systemData = A.Fake<SystemData>();
            systemData.ToggleFixedCurrentDateUsage(false);

            A.CallTo(() => systemDataDataAccess.Get()).Returns(systemData);
            A.CallTo(() => aatfDataAccess.GetAatfsForOrganisation(A<Guid>._)).Returns(aatfs);

            await handler.HandleAsync(A.Dummy<GetAatfByOrganisation>());

            foreach (var aatfData in aatfs)
            {
                A.CallTo(() => mapper.Map(A<AatfWithSystemDateMapperObject>.That.Matches(a =>
                    Equals(a.Aatf, aatfData) && a.SystemDateTime == date))).MustHaveHappenedOnceExactly();
            }

            SystemTime.Unfreeze();
        }

        [Fact]
        public async void HandleAsync_GivenMappedAatfData_AatfDataShouldBeReturn()
        {
            var aatfDatas = new List<AatfData>()
            {
                A.Fake<AatfData>(),
                A.Fake<AatfData>()
            }.ToArray();

            A.CallTo(() => aatfDataAccess.GetAatfsForOrganisation(A<Guid>._)).Returns(Aatfs());
            A.CallTo(() => mapper.Map(A<AatfWithSystemDateMapperObject>._)).ReturnsNextFromSequence(aatfDatas);

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
