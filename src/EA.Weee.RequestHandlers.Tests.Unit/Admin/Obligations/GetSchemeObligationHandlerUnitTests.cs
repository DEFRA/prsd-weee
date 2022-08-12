namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Admin.Obligation;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Admin.Obligations;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemeObligationHandlerUnitTests : SimpleUnitTestBase
    {
        private GetSchemeObligationHandler handler;
        private readonly GetSchemeObligation request;
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly IMapper mapper;

        public GetSchemeObligationHandlerUnitTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            obligationDataAccess = A.Fake<IObligationDataAccess>();
            mapper = A.Fake<IMapper>();

            request = new GetSchemeObligation(TestFixture.Create<CompetentAuthority>(), TestFixture.Create<int>());

            handler = new GetSchemeObligationHandler(authorization, mapper, obligationDataAccess, commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetSchemeObligationHandler(authorization, mapper, obligationDataAccess, commonDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NotAnAdminUser_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyAnyRole().Build();

            handler = new GetSchemeObligationHandler(authorization, mapper, obligationDataAccess, commonDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_InternalAccess_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_UserInAdminRole_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureUserInRole(Roles.InternalAdmin)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_CompetentAuthorityShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.Authority)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SchemeObligationDataShouldBeRetrieved()
        {
            //arrange
            var authority = TestFixture.Create<UKCompetentAuthority>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._))
                .Returns(authority);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(A<UKCompetentAuthority>.That.IsSameAs(authority), request.ComplianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndSchemeObligationData_ReturnedDataShouldBeMapped()
        {
            //arrange
            var schemes = TestFixture.CreateMany<Scheme>().ToList();
            A.CallTo(() => obligationDataAccess.GetObligationSchemeData(A<UKCompetentAuthority>._, A<int>._)).Returns(schemes);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<List<Scheme>, List<SchemeObligationData>>(schemes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMappedReturnedData_MappedReturnedDataShouldBeReturned()
        {
            //arrange
            var returnData = TestFixture.CreateMany<SchemeObligationData>().ToList();
            A.CallTo(() => mapper.Map<List<Scheme>, List<SchemeObligationData>>(A<List<Scheme>>._)).Returns(returnData);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeEquivalentTo(returnData);
        }
    }
}
