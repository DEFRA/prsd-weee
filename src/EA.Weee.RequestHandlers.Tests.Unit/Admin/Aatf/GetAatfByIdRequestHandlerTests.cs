namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    //using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
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

    public class GetAatfByIdRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly AatfMap mapper;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly GetAatfInfoByAatfIdRequestHandler handler;

        public GetAatfByIdRequestHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserWithAllRights();
            mapper = new AatfMap(A.Fake<IMap<Domain.UKCompetentAuthority, UKCompetentAuthorityData>>(),
                A.Fake<IMap<Domain.AatfReturn.AatfStatus, Core.AatfReturn.AatfStatus>>(),
                A.Fake<IMap<Domain.AatfReturn.AatfSize, Core.AatfReturn.AatfSize>>(),
                A.Fake<IMap<AatfAddress, AatfAddressData>>());
            dataAccess = A.Dummy<IGetAatfsDataAccess>();
            
            handler = new GetAatfInfoByAatfIdRequestHandler(authorization, mapper, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization unuthorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetAatfInfoByAatfIdRequestHandler handler = new GetAatfInfoByAatfIdRequestHandler(
                unuthorization,
                A.Dummy<IMap<Aatf, AatfData>>(),
                A.Dummy<IGetAatfsDataAccess>());

            // Act
            Func<Task> testCode = async () => await handler.HandleAsync(A.Dummy<GetAatfById>());

            // Asert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_ProvideAatfId_AatfShouldBeMapped()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string name = "fake name";
            string approvalNumber = "1234";

            Aatf aatf = new Aatf(name, A.Fake<Domain.UKCompetentAuthority>(), approvalNumber, A.Fake<Domain.AatfReturn.AatfStatus>(), A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<Domain.AatfReturn.AatfSize>(), DateTime.Now);
            A.CallTo(() => dataAccess.GetAatfById(id)).Returns(aatf);

            // Act
            var result = await handler.HandleAsync(new GetAatfById(A.Dummy<Guid>()));

            // Assert
            result.ApprovalNumber = approvalNumber;
            result.Name = name;
        }

        [Fact]
        public async Task HandleAsync_GivenGetAatfsReturnRequest_DataAccessFetchIsCalled()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            await handler.HandleAsync(A.Dummy<GetAatfById>());
            A.CallTo(() => dataAccess.GetAatfById(A.Dummy<Guid>())).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
