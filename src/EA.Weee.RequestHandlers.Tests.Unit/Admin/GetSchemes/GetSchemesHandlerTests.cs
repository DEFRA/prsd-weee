namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSchemes
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain;
    using Domain.Obligation;
    using Domain.Scheme;
    using FakeItEasy;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetSchemes;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemesHandlerTests
    {
        /// <summary>
        /// This test ensures that a user who cannot access th internal area cannot use
        /// the GetSchemes handler and that any attempt to do so will result in a
        /// SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WhenUserCannotAccessInternalArea_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetSchemesHandler handler = new GetSchemesHandler(
                authorization,
                A.Dummy<IMap<Scheme, SchemeData>>(),
                A.Dummy<IGetSchemesDataAccess>());

            // Act
            Func<Task> testCode = async () => await handler.HandleAsync(A.Dummy<GetSchemes>());

            // Asert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that when the filter on the GetSchemes request is set to "Approved"
        /// then only schemes with a scheme status of "Approved" are returned in the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithFilterSetToApproved_ReturnsSchemesWithStatusOfApproved()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IMap<Scheme, SchemeData> schemeMap = new SchemeMap(new UKCompetentAuthorityMap());

            Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

            Scheme schemePending = new Scheme(organisation);
            schemePending.UpdateScheme("Scheme Pending", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());

            Scheme schemeApproved = new Scheme(organisation);
            schemeApproved.UpdateScheme("Scheme Approved", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeApproved.SetStatus(SchemeStatus.Approved);

            Scheme schemeRejected = new Scheme(organisation);
            schemeRejected.UpdateScheme("Scheme Rejected", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeRejected.SetStatus(SchemeStatus.Rejected);

            Scheme schemeWithdrawn = new Scheme(organisation);
            schemeWithdrawn.UpdateScheme("Scheme Withdrawn", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeWithdrawn.SetStatus(SchemeStatus.Approved);
            schemeWithdrawn.SetStatus(SchemeStatus.Withdrawn);

            IGetSchemesDataAccess dataAccess = A.Fake<IGetSchemesDataAccess>();
            A.CallTo(() => dataAccess.GetSchemes()).Returns(
                new List<Scheme>()
                {
                    schemePending,
                    schemeApproved,
                    schemeRejected,
                    schemeWithdrawn
                });

            GetSchemesHandler handler = new GetSchemesHandler(
                authorization,
                schemeMap,
                dataAccess);

            // Act
            GetSchemes request = new GetSchemes(GetSchemes.FilterType.Approved);

            List<SchemeData> results = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(1, results.Count);

            Assert.Contains(results, r => r.SchemeName == "Scheme Approved");
        }

        /// <summary>
        /// This test ensures that when the filter on the GetSchemes request is set to "ApprovedOrWithdrawn"
        /// then only schemes with a scheme status of "Approved" or "Withdrawn" are returned
        /// in the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithFilterSetToApprovedOrWithdrawn_ReturnsSchemesWithStatusOfApprovedOrWithdrawn()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IMap<Scheme, SchemeData> schemeMap = new SchemeMap(new UKCompetentAuthorityMap());

            Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

            Scheme schemePending = new Scheme(organisation);
            schemePending.UpdateScheme("Scheme Pending", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());

            Scheme schemeApproved = new Scheme(organisation);
            schemeApproved.UpdateScheme("Scheme Approved", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeApproved.SetStatus(SchemeStatus.Approved);

            Scheme schemeRejected = new Scheme(organisation);
            schemeRejected.UpdateScheme("Scheme Rejected", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeRejected.SetStatus(SchemeStatus.Rejected);

            Scheme schemeWithdrawn = new Scheme(organisation);
            schemeWithdrawn.UpdateScheme("Scheme Withdrawn", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            schemeWithdrawn.SetStatus(SchemeStatus.Approved);
            schemeWithdrawn.SetStatus(SchemeStatus.Withdrawn);

            IGetSchemesDataAccess dataAccess = A.Fake<IGetSchemesDataAccess>();
            A.CallTo(() => dataAccess.GetSchemes()).Returns(
                new List<Scheme>()
                {
                    schemePending,
                    schemeApproved,
                    schemeRejected,
                    schemeWithdrawn
                });

            GetSchemesHandler handler = new GetSchemesHandler(
                authorization,
                schemeMap,
                dataAccess);

            // Act
            GetSchemes request = new GetSchemes(GetSchemes.FilterType.ApprovedOrWithdrawn);

            List<SchemeData> results = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);

            Assert.Contains(results, r => r.SchemeName == "Scheme Approved");
            Assert.Contains(results, r => r.SchemeName == "Scheme Withdrawn");
        }

        /// <summary>
        /// This test ensures that the results are always returned ordered by the name
        /// of the scheme from A to Z.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_ReturnsSchemesOrderedBySchemeName()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IMap<Scheme, SchemeData> schemeMap = new SchemeMap(new UKCompetentAuthorityMap());

            Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");

            Scheme scheme1 = new Scheme(organisation);
            scheme1.UpdateScheme("Scheme C", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme1.SetStatus(SchemeStatus.Approved);

            Scheme scheme2 = new Scheme(organisation);
            scheme2.UpdateScheme("Scheme A", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme2.SetStatus(SchemeStatus.Approved);

            Scheme scheme3 = new Scheme(organisation);
            scheme3.UpdateScheme("Scheme D", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme3.SetStatus(SchemeStatus.Approved);

            Scheme scheme4 = new Scheme(organisation);
            scheme4.UpdateScheme("Scheme B", "WEE/11AAAA11/SCH", "WEE1234567", ObligationType.Both, A.Dummy<UKCompetentAuthority>());
            scheme4.SetStatus(SchemeStatus.Approved);

            IGetSchemesDataAccess dataAccess = A.Fake<IGetSchemesDataAccess>();
            A.CallTo(() => dataAccess.GetSchemes()).Returns(
                new List<Scheme>()
                {
                    scheme1,
                    scheme2,
                    scheme3,
                    scheme4
                });

            GetSchemesHandler handler = new GetSchemesHandler(
                authorization,
                schemeMap,
                dataAccess);

            // Act
            GetSchemes request = new GetSchemes(GetSchemes.FilterType.Approved);

            List<SchemeData> results = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(4, results.Count);

            Assert.Collection(results,
                r => Assert.Equal("Scheme A", r.SchemeName),
                r => Assert.Equal("Scheme B", r.SchemeName),
                r => Assert.Equal("Scheme C", r.SchemeName),
                r => Assert.Equal("Scheme D", r.SchemeName));
        }
    }
}
