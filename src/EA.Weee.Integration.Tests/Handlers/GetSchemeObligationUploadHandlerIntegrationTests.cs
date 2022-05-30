namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Admin.Obligation;
    using Domain.Error;
    using Domain.Obligation;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    public class GetSchemeUploadObligationHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetASchemeUploadObligation : GetSchemeUploadObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var errors = new List<ObligationUploadError>()
                {
                    new ObligationUploadError(ObligationUploadErrorType.File, "file error"),
                    new ObligationUploadError(ObligationUploadErrorType.Data, "scheme name", "scheme id", "data error")
                };
                obligationUpload = ObligationUploadDbSetup.Init().WithErrors(errors).Create();
                
                request = new GetSchemeObligationUpload(obligationUpload.Id);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(obligationUpload.Id);
            };

            private readonly It shouldHaveReturnedObligationData = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedObligationDataWithErrors = () =>
            {
                result.ErrorData.Should().Contain(e =>
                    e.ErrorType == SchemeObligationUploadErrorType.File && e.Description.Equals("file error"));

                result.ErrorData.Should().Contain(e =>
                    e.ErrorType == SchemeObligationUploadErrorType.Data && e.Description.Equals("data error") && e.Scheme.Equals("scheme name") && e.SchemeIdentifier.Equals("scheme id"));
            };
        }

        public class GetSchemeUploadObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemeObligationUpload, SchemeObligationUploadData> handler;
            protected static GetSchemeObligationUpload request;
            protected static Fixture fixture;
            protected static SchemeObligationUploadData result;
            protected static ObligationUpload obligationUpload;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalAdminUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetSchemeObligationUpload, SchemeObligationUploadData>>();

                return setup;
            }
        }
    }
}
