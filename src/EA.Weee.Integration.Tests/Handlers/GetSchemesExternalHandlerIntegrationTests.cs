namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Core.Scheme;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetSchemesExternalHandlerIntegrationTest : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemesIncludingWithdrawn : GetSchemesExternalHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation1 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation1.Id).Create();
                includedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation1.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create());

                var organisation2 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation2.Id).Create();
                includedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation2.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .WithStatus(SchemeStatus.Withdrawn)
                    .Create());

                var organisation3 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation3.Id).Create();
                notIncludedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation3.Id)
                    .WithStatus(SchemeStatus.Pending)
                    .Create());

                var organisation4 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation4.Id).Create();
                notIncludedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation4.Id)
                    .WithStatus(SchemeStatus.Pending)
                    .WithStatus(SchemeStatus.Rejected)
                    .Create());
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(new GetSchemesExternal(true))).Result;
            };

            private readonly It shouldHaveExpectedResults = () =>
            {
                result.Should().HaveCount(2);
            };

            private readonly It shouldContainExpectedSchemes = () =>
            {
                foreach (var includedScheme in includedSchemes)
                {
                    result.Should().Contain(s => s.Id.Equals(includedScheme.Id));
                }
            };

            private readonly It shouldNotContainNotExpectedSchemes = () =>
            {
                foreach (var excludedScheme in notIncludedSchemes)
                {
                    result.Should().NotContain(s => s.Id.Equals(excludedScheme.Id));
                }
            };

            private readonly It shouldHaveMappedTheSchemes = () =>
            {
                foreach (var includedScheme in includedSchemes)
                {
                    var expectedResult = result.First(r => r.Id.Equals(includedScheme.Id));
                    var scheme = Query.GetSchemeById(includedScheme.Id);
                    expectedResult.ShouldMapScheme(scheme);
                }
            };

            private readonly It shouldHaveOrderedResults = () =>
            {
                result.Should().BeInAscendingOrder(s => s.SchemeName);
            };
        }

        [Component]
        public class WhenIGetSchemesNotIncludingWithdrawn : GetSchemesExternalHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation1 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation1.Id).Create();
                includedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation1.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .Create());

                var organisation2 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation2.Id).Create();
                notIncludedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation2.Id)
                    .WithStatus(SchemeStatus.Approved)
                    .WithStatus(SchemeStatus.Withdrawn)
                    .Create());

                var organisation3 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation3.Id).Create();
                notIncludedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation3.Id)
                    .WithStatus(SchemeStatus.Pending)
                    .Create());

                var organisation4 = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation4.Id).Create();
                notIncludedSchemes.Add(SchemeDbSetup.Init().WithOrganisation(organisation4.Id)
                    .WithStatus(SchemeStatus.Pending)
                    .WithStatus(SchemeStatus.Rejected)
                    .Create());
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(new GetSchemesExternal(false))).Result;
            };

            private readonly It shouldHaveExpectedResults = () =>
            {
                result.Should().HaveCount(1);
            };

            private readonly It shouldContainExpectedSchemes = () =>
            {
                foreach (var includedScheme in includedSchemes)
                {
                    result.Should().Contain(s => s.Id.Equals(includedScheme.Id));
                }
            };

            private readonly It shouldNotContainNotExpectedSchemes = () =>
            {
                foreach (var excludedScheme in notIncludedSchemes)
                {
                    result.Should().NotContain(s => s.Id.Equals(excludedScheme.Id));
                }
            };

            private readonly It shouldHaveMappedTheSchemes = () =>
            {
                foreach (var includedScheme in includedSchemes)
                {
                    var expectedResult = result.First(r => r.Id.Equals(includedScheme.Id));
                    var scheme = Query.GetSchemeById(includedScheme.Id);
                    expectedResult.ShouldMapScheme(scheme);
                }
            };

            private readonly It shouldHaveOrderedResults = () =>
            {
                result.Should().BeInAscendingOrder(s => s.SchemeName);
            };
        }

        public class WhenIGetSchemesAddressWhereUserIsNotAuthorisedToExternalArea : GetSchemesExternalHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                handler = Container.Resolve<IRequestHandler<GetSchemesExternal, List<SchemeData>>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(new GetSchemesExternal(false)));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetSchemesExternalHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static List<SchemeData> result;
            protected static List<Scheme> includedSchemes;
            protected static List<Scheme> notIncludedSchemes;
            protected static IRequestHandler<GetSchemesExternal, List<SchemeData>> handler;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetData: true)
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<GetSchemesExternal, List<SchemeData>>>();

                result = new List<SchemeData>();
                includedSchemes = new List<Scheme>();
                notIncludedSchemes = new List<Scheme>();
            }
        }
    }
}
