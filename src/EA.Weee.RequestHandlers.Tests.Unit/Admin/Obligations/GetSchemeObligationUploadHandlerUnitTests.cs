namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Admin.Obligation;
    using DataAccess.DataAccess;
    using Domain.Obligation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
    using Requests.Admin.Obligations;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemeObligationUploadHandlerUnitTests
    {
        private GetSchemeObligationUploadHandler handler;
        private readonly GetSchemeObligationUpload request;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly Fixture fixture;

        public GetSchemeObligationUploadHandlerUnitTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();

            fixture = new Fixture();
            request = new GetSchemeObligationUpload(fixture.Create<Guid>());

            handler = new GetSchemeObligationUploadHandler(authorization, genericDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetSchemeObligationUploadHandler(authorization, genericDataAccess, mapper);

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

            handler = new GetSchemeObligationUploadHandler(authorization, genericDataAccess, mapper);
            
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
        public async Task HandleAsync_GivenRequest_ObligationUploadShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => genericDataAccess.GetById<ObligationUpload>(request.ObligationUploadId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationUploadNotFound_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => genericDataAccess.GetById<ObligationUpload>(A<Guid>._)).Returns((ObligationUpload)null);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationUpload_ReturnedDataShouldBeMapped()
        {
            //arrange
            var obligationUpload = fixture.Create<ObligationUpload>();
            A.CallTo(() => genericDataAccess.GetById<ObligationUpload>(A<Guid>._)).Returns(obligationUpload);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<ObligationUpload, SchemeObligationUploadData>(obligationUpload))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMappedReturnedData_MappedReturnedDataShouldBeReturned()
        {
            //arrange
            var returnData = fixture.Create<SchemeObligationUploadData>();
            A.CallTo(() => mapper.Map<ObligationUpload, SchemeObligationUploadData>(A<ObligationUpload>._))
                .Returns(returnData);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(returnData);
        }
    }
}
