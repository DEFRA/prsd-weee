﻿namespace EA.Weee.Integration.Tests.Handlers
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

                request = new GetObligationComplianceYears(CompetentAuthority.England);
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

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetAdminRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetObligationComplianceYears, List<int>>>();

                return setup;
            }
        }
    }
}
