namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSchemes
{
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetSchemes;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetMemberRegistrationSchemesByComplianceYearHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISchemeDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;
        private GetMemberRegistrationSchemesByComplianceYearHandler handler;

        public GetMemberRegistrationSchemesByComplianceYearHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<ISchemeDataAccess>();
            this.schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            this.context = A.Fake<WeeeContext>();

            handler = new GetMemberRegistrationSchemesByComplianceYearHandler(authorization, schemeMap, context, dataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetMemberRegistrationSchemesByComplianceYearHandler(authorization, schemeMap, context, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetMemberRegistrationSchemesByComplianceYear>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_InternalAccess_NotThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            handler = new GetMemberRegistrationSchemesByComplianceYearHandler(authorization, schemeMap, context, dataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetMemberRegistrationSchemesByComplianceYear>());

            await action.Should().NotThrowAsync<SecurityException>();
        }
    }
}
