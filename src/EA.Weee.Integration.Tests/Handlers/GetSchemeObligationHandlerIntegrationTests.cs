namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Admin.Obligation;
    using Core.Helpers;
    using Core.Shared;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class GetSchemeObligationHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemeObligations : GetSchemeObligationHandlerIntegrationTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme scheme3;
            private static Scheme scheme4;
            private static Scheme scheme5;
            private static Scheme scheme6;
            private static Scheme scheme7;
            private static Scheme notMatchingScheme;

            private readonly Establish context = () =>
            {
                LocalSetup();

                var matchingAuthority = Query.GetCompetentAuthorityByName("Environment Agency");
                var nonMatchingAuthority = Query.GetCompetentAuthorityByName("Northern Ireland Environment Agency");
                var organisation = OrganisationDbSetup.Init().Create();
                scheme1 = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                scheme2 = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithOrganisation(organisation.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create();

                //not matching authority scheme
                notMatchingScheme = SchemeDbSetup.Init()
                    .WithAuthority(nonMatchingAuthority.Id)
                    .WithOrganisation(organisation.Id).Create();

                scheme3 = SchemeDbSetup.Init()
                    .WithAuthority(nonMatchingAuthority.Id)
                    .WithStatus(SchemeStatus.Pending)
                    .WithOrganisation(organisation.Id).Create();

                scheme4 = SchemeDbSetup.Init()
                    .WithAuthority(nonMatchingAuthority.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .WithStatus(SchemeStatus.Withdrawn)
                    .WithOrganisation(organisation.Id).Create();

                scheme5 = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithStatus(SchemeStatus.Rejected)
                    .WithOrganisation(organisation.Id).Create();

                scheme6 = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithStatus(SchemeStatus.Rejected)
                    .WithOrganisation(organisation.Id).Create();

                scheme7 = SchemeDbSetup.Init()
                    .WithAuthority(matchingAuthority.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .WithStatus(SchemeStatus.Withdrawn)
                    .WithOrganisation(organisation.Id).Create();

                var obligationUpload = ObligationUploadDbSetup.Init().Create();

                // should be returned
                var amounts1 = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 1000),
                    new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, null)
                };

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme1.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithObligationAmounts(amounts1)
                    .WithComplianceYear(2022).Create();
                
                ObligationSchemeDbSetup.Init()
                    .WithScheme(notMatchingScheme.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2022).Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme3.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2022).Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme4.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2022).Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme5.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2022).Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme6.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2023).Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme7.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2022).Create();

                // should not be returned not matching compliance year
                ObligationSchemeDbSetup.Init().WithScheme(scheme1.Id).WithObligationUpload(obligationUpload.Id).WithComplianceYear(2021).Create();

                request = new GetSchemeObligation(CompetentAuthority.England, 2022);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedObligationData = () =>
            {
                result.Should().NotBeNull();
                var schemeData = result.Where(s => s.SchemeName.Equals(scheme1.SchemeName));
                schemeData.Count().Should().Be(1);
                schemeData.ElementAt(0).UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeData.ElementAt(0).SchemeObligationAmountData.Count.Should().Be(2);
                schemeData.ElementAt(0).SchemeObligationAmountData.Should().Contain(o =>
                    o.Obligation == 1000 && o.Category.ToInt() == WeeeCategory.LargeHouseholdAppliances.ToInt());
                schemeData.ElementAt(0).SchemeObligationAmountData.Should().Contain(o =>
                    o.Obligation == null && o.Category.ToInt() == WeeeCategory.SmallHouseholdAppliances.ToInt());

                schemeData = result.Where(s => s.SchemeName.Equals(scheme2.SchemeName));
                schemeData.Count().Should().Be(1);
                schemeData.ElementAt(0).UpdatedDate.Should().BeNull();
                schemeData.ElementAt(0).SchemeObligationAmountData.Should().BeEmpty();
            };

            private readonly It shouldHaveReturnedTheCorrectSchemes = () =>
            {
                result.Should().Contain(s => s.SchemeId == scheme1.Id);
                result.Should().Contain(s => s.SchemeId == scheme2.Id);
                result.Should().NotContain(s => s.SchemeId == scheme3.Id);
                result.Should().Contain(s => s.SchemeId == scheme5.Id);
                result.Should().Contain(s => s.SchemeId == scheme7.Id);
                result.Should().NotContain(s => s.SchemeId == scheme6.Id); // not correct status but has data in anothe ryear
                result.Should().NotContain(s => s.SchemeId == notMatchingScheme.Id); // not matching authority
                result.Should().NotContain(s => s.SchemeId == scheme4.Id); // not matching authority
            };
        }

        public class GetSchemeObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemeObligation, List<SchemeObligationData>> handler;
            protected static GetSchemeObligation request;
            protected static Fixture fixture;
            protected static List<SchemeObligationData> result;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC(true)
                    .WithTestData()
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetSchemeObligation, List<SchemeObligationData>>>();

                return setup;
            }
        }
    }
}
