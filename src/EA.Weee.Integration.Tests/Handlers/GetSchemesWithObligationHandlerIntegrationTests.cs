namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Scheme;
    using Core.Shared;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class GetSchemesWithObligationHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemesWithObligations : GetSchemesWithObligationHandlerTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme schemeNotMatchingComplianceYear;

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

                schemeNotMatchingComplianceYear = SchemeDbSetup.Init()
                    .WithAuthority(authority1.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                var obligationUploadCurrentYear = ObligationUploadDbSetup.Init().Create();
                var obligationUploadNotMatchingYear = ObligationUploadDbSetup.Init().Create();

                ObligationSchemeDbSetup.Init().WithScheme(scheme1.Id)
                    .WithObligationUpload(obligationUploadCurrentYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(scheme2.Id)
                    .WithObligationUpload(obligationUploadCurrentYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(schemeNotMatchingComplianceYear.Id)
                    .WithObligationUpload(obligationUploadNotMatchingYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year - 1)
                    .Create();

                ObligationSchemeDbSetup.Init().WithScheme(schemeNotMatchingComplianceYear.Id)
                    .WithObligationUpload(obligationUploadNotMatchingYear.Id)
                    .WithComplianceYear(SystemTime.UtcNow.Year + 1)
                    .Create();

                request = new GetSchemesWithObligation(SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedSchemesWithObligations = () =>
            {
                result.Should().NotBeNull();
                result.Count.Should().Be(2);
                result.Should().Contain(s => s.Id == scheme1.Id);
                result.Should().Contain(s => s.Id == scheme2.Id);
                result.Should().NotContain(s => s.Id == schemeNotMatchingComplianceYear.Id);
                result.Should().OnlyHaveUniqueItems();
            };
        }

        public class GetSchemesWithObligationHandlerTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemesWithObligation, List<SchemeData>> handler;
            protected static GetSchemesWithObligation request;
            protected static Fixture fixture;
            protected static List<SchemeData> result;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData(true)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetSchemesWithObligation, List<SchemeData>>>();

                return setup;
            }
        }
    }
}
