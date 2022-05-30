namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Core.Shared;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    public class SubmitSchemeObligationRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenISubmitSchemeObligation : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
                var csvHeader =
                    @"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                WEE / TE0092ST / SCH,George's PCS,,,,,,,,,,,,,,
                WEE / TE0095ST / SCH,Emily's PCS,,,,,,,,,,,,,,";

                var fileInfo = new FileInfo("file name", Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                result.Should().NotBeEmpty();
            };
        }

        public class SubmitSchemeObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<SubmitSchemeObligation, Guid> handler;
            protected static SubmitSchemeObligation request;
            protected static Guid result;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<SubmitSchemeObligation, Guid>>();

                return setup;
            }
        }
    }
}
