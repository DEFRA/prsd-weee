//namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
//{
//    using Autofac;
//    using AutoFixture;
//    using Base;
//    using EA.Weee.Core.Organisations;
//    using EA.Weee.Core.Organisations.Base;
//    using EA.Weee.Core.Shared;
//    using EA.Weee.Domain;
//    using EA.Weee.Domain.Organisation;
//    using EA.Weee.Domain.Producer;
//    using EA.Weee.Integration.Tests.Builders;
//    using EA.Weee.Requests.Organisations.DirectRegistrant;
//    using FluentAssertions;
//    using Newtonsoft.Json;
//    using NUnit.Specifications.Categories;
//    using Prsd.Core.Autofac;
//    using Prsd.Core.Mediator;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Security;

//    public class EditOrganisationDetailsRequestHandlerIntegrationTests : IntegrationTestBase
//    {
//        [Component]
//        public class WhenIUpdateOrganisationDetails : EditOrganisationDetailsRequestHandlerIntegrationTestBase
//        {
//            private readonly Establish context = () =>
//            {
//                LocalSetup();
//            };

//            private readonly Because of = () =>
//            {
//                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
//            };

//            private readonly It shouldHappen = () =>
//            {
//                var entity = Query.GetOrganisationTransactionForUser(UserId.ToString());
//            };
//        }

//        [Component]
//        public class WhenUserIsNotAuthorised : EditOrganisationDetailsRequestHandlerIntegrationTestBase
//        {
//            protected static IRequestHandler<EditOrganisationDetailsRequest, bool> authHandler;

//            private readonly Establish context = () =>
//            {
//                SetupTest(IocApplication.RequestHandler)
//                    .WithDefaultSettings();

//                authHandler = Container.Resolve<IRequestHandler<EditOrganisationDetailsRequest, bool>>();
//            };

//            private readonly Because of = () =>
//            {
//                CatchExceptionAsync(() => authHandler.HandleAsync(request));
//            };

//            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
//        }

//        public class EditOrganisationDetailsRequestHandlerIntegrationTestBase : WeeeContextSpecification
//        {
//            protected static IRequestHandler<EditOrganisationDetailsRequest, bool> handler;
//            protected static Fixture fixture;
//            protected static Domain.Producer.DirectRegistrant directRegistrant;
//            protected static EditOrganisationDetailsRequest request;
//            protected static bool result;
//            protected static Country country;

//            public static IntegrationTestSetupBuilder LocalSetup()
//            {
//                var setup = SetupTest(IocApplication.RequestHandler)
//                    .WithIoC()
//                    .WithTestData()
//                .WithExternalUserAccess();

//                directRegistrant = DirectRegistrantDbSetup.Init()
//                    .WithSubmissions()
//                    .Create();

//                handler = Container.Resolve<IRequestHandler<EditOrganisationDetailsRequest, bool>>();

//                fixture = new Fixture();

//                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

//                var address = fixture.Build<AddressData>().With(a => a.CountryId).Create();

//                request = new EditOrganisationDetailsRequest(directRegistrant.Id, "New company name",
//                    "New trading name", address, "New brand names");

//                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

//                return setup;
//            }
//        }
//    }
//}
