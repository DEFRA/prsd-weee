namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using Xunit;

    public class AddAatfRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;

        public AddAatfRequestHandlerTests()
        {
            this.authorization = AuthorizationBuilder.CreateUserWithAllRights();
            this.addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            this.contactMapper = A.Fake<IMap<AatfContactData, AatfContact>>();
            this.dataAccess = A.Fake<IGenericDataAccess>();
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);
            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, this.dataAccess, addressMapper, contactMapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            UserManager<ApplicationUser> userManager = A.Fake<UserManager<ApplicationUser>>();

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, this.dataAccess, addressMapper, contactMapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_ValidInput_AddsAatf()
        {
            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();

            AatfData aatf = new AatfData(Guid.NewGuid(), "name", "approval number", (Int16)2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now);

            Guid aatfId = Guid.NewGuid();

            AddAatfRequestHandler handler = new AddAatfRequestHandler(authorization, this.dataAccess, addressMapper, contactMapper);

            AddAatf request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid()
            };

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(
                c => c.Name == aatf.Name
                && c.AatfStatus == aatf.AatfStatus
                && c.ApprovalDate == aatf.ApprovalDate
                && c.ApprovalNumber == aatf.ApprovalNumber
                && c.CompetentAuthorityId == aatf.CompetentAuthority.Id
                && c.Name == aatf.Name
                && c.SiteAddress.Id == aatf.SiteAddress.Id
                && c.Size == aatf.Size
                && c.ComplianceYear == aatf.ComplianceYear))).Returns(aatfId);

            bool result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(
                c => c.Name == aatf.Name
                && c.ApprovalNumber == aatf.ApprovalNumber
                && c.CompetentAuthorityId == aatf.CompetentAuthority.Id
                && c.Name == aatf.Name
                && c.SiteAddress.Id == aatf.SiteAddress.Id
                && Enumeration.FromValue<Domain.AatfReturn.AatfSize>(c.Size.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfSize>(aatf.Size.Value)
                && Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(c.AatfStatus.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(aatf.AatfStatus.Value)
                && c.ApprovalDate == aatf.ApprovalDate
                && c.ComplianceYear == aatf.ComplianceYear))).MustHaveHappened(Repeated.Exactly.Once);

            result.Should().Be(true);
        }
    }
}