namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class GetObligationComplianceYearsHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemeObligationYears : GetObligationComplianceYearsHandlerTestBase
        {
            private static Scheme schemeMatchingAuthority;
            private static Scheme schemeNotMatchingAuthority;

            private readonly Establish context = () =>
            {
                LocalSetup();

                var matchingAuthority = Query.GetCompetentAuthorityByName("Environment Agency");
                var nonMatchingAuthority = Query.GetCompetentAuthorityByName("Northern Ireland Environment Agency");
                var organisation = OrganisationDbSetup.Init().Create();
                schemeMatchingAuthority = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                schemeNotMatchingAuthority = SchemeDbSetup.Init()
                    .WithAuthority(nonMatchingAuthority.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                var obligationUploadCurrentYear = ObligationUploadDbSetup.Init().Create();
                var obligationUploadPreviousYear = ObligationUploadDbSetup.Init().Create();
                var obligationUploadNotMatchingAuthority = ObligationUploadDbSetup.Init().Create();

                ObligationSchemeDbSetup.Init().WithScheme(schemeMatchingAuthority.Id)
                    .WithObligationUpload(obligationUploadCurrentYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(schemeMatchingAuthority.Id)
                    .WithObligationUpload(obligationUploadPreviousYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year - 1)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(schemeNotMatchingAuthority.Id)
                    .WithObligationUpload(obligationUploadNotMatchingAuthority.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year - 2)
                    .Create();

                request = new GetObligationComplianceYears(CompetentAuthority.England, false);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedObligationYears = () =>
            {
                result.Should().NotBeNull();
                result.Count.Should().Be(2);
                result.Should().BeEquivalentTo(new List<int>() { SystemTime.UtcNow.Year, SystemTime.UtcNow.Year - 1 });
            };
        }

        [Component]
        public class WhenIGetSchemeObligationYearsWithoutSpecifyingAnAuthority : GetObligationComplianceYearsHandlerTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;

            private readonly Establish context = () =>
            {
                LocalSetup();

                var authority1 = Query.GetCompetentAuthorityByName("Environment Agency");
                var authority2 = Query.GetCompetentAuthorityByName("Northern Ireland Environment Agency");
                var organisation = OrganisationDbSetup.Init().Create();
                scheme1 = SchemeDbSetup.Init()
                    .WithAuthority(authority1.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                scheme2 = SchemeDbSetup.Init()
                    .WithAuthority(authority2.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                var obligationUploadCurrentYear = ObligationUploadDbSetup.Init().Create();
                var obligationUploadPreviousYear = ObligationUploadDbSetup.Init().Create();
                var obligationUploadNotMatchingAuthority = ObligationUploadDbSetup.Init().Create();

                ObligationSchemeDbSetup.Init().WithScheme(scheme1.Id)
                    .WithObligationUpload(obligationUploadCurrentYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(scheme1.Id)
                    .WithObligationUpload(obligationUploadPreviousYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year - 1)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(scheme2.Id)
                    .WithObligationUpload(obligationUploadNotMatchingAuthority.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year - 2)
                    .Create();

                request = new GetObligationComplianceYears(null, false);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedObligationYears = () =>
            {
                result.Should().NotBeNull();
                result.Count.Should().Be(3);
                result.Should().BeEquivalentTo(new List<int>() { SystemTime.UtcNow.Year, SystemTime.UtcNow.Year - 1, SystemTime.UtcNow.Year - 2 });
            };
        }

        public class GetObligationComplianceYearsHandlerTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetObligationComplianceYears, List<int>> handler;
            protected static GetObligationComplianceYears request;
            protected static Fixture fixture;
            protected static List<int> result;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData(true)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetObligationComplianceYears, List<int>>>();

                return setup;
            }
        }
    }
}
