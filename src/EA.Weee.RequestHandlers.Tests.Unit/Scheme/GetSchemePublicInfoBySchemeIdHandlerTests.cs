namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme;
    using RequestHandlers.Scheme.GetSchemePublicInfo;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemePublicInfoBySchemeIdHandlerTests
    {
        [Fact]
        public async void GetSchemeByIdHandler_HappyPath_ReturnsSchemeData()
        {
            // Arrage
            var dataAccess = A.Fake<ISchemeDataAccess>();
            var scheme = A.Fake<Scheme>();

            A.CallTo(() => scheme.Id).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.SchemeName).Returns("scheme");
            A.CallTo(() => scheme.ApprovalNumber).Returns("approval");
            A.CallTo(() => dataAccess.GetSchemeOrDefault(scheme.Id)).Returns(scheme);

            var handler = new GetSchemePublicInfoBySchemeIdHandler(dataAccess);
            var request = new GetSchemePublicInfoBySchemeId(scheme.Id);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.OrganisationId.Should().Be(scheme.OrganisationId);
            result.Name.Should().Be(scheme.SchemeName);
            result.ApprovalNo.Should().Be(scheme.ApprovalNumber);
            result.SchemeId.Should().Be(scheme.Id);
        }

        [Fact]
        public async void GetSchemeByIdHandler_WithUnknownId_ThrowsArgumentException()
        {
            var badSchemeId = Guid.NewGuid();

            var dataAccess = A.Fake<ISchemeDataAccess>();
            A.CallTo(() => dataAccess.GetSchemeOrDefault(badSchemeId)).Returns((Scheme)null);

            var handler = new GetSchemePublicInfoBySchemeIdHandler(dataAccess);
            var request = new GetSchemePublicInfoBySchemeId(badSchemeId);

            // Act
            Func<Task<SchemePublicInfo>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
