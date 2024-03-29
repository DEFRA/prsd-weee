﻿namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Admin.Obligation;
    using Core.Shared;
    using Domain.Error;
    using Domain.Obligation;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    public class GetSchemeUploadObligationHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetASchemeUploadObligation : GetSchemeUploadObligationHandlerIntegrationTestBase
        {
            private const string FileError = "file error";
            private const string DataError = "data error";
            private const string SchemeName = "scheme name";
            private const string SchemeId = "scheme id";

            private readonly Establish context = () =>
            {
                LocalSetup();

                var errors = new List<ObligationUploadError>()
                {
                    new ObligationUploadError(ObligationUploadErrorType.File, FileError),
                    new ObligationUploadError(ObligationUploadErrorType.Data, SchemeName, SchemeId, DataError)
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
                result.Should().Contain(e =>
                    e.ErrorType == SchemeObligationUploadErrorType.File && e.Description.Equals(FileError));

                result.Should().Contain(e =>
                    e.ErrorType == SchemeObligationUploadErrorType.Data && e.Description.Equals(DataError) && e.Scheme.Equals(SchemeName) && e.SchemeIdentifier.Equals(SchemeId));
            };
        }

        public class GetSchemeUploadObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemeObligationUpload, List<SchemeObligationUploadErrorData>> handler;
            protected static GetSchemeObligationUpload request;
            protected static Fixture fixture;
            protected static List<SchemeObligationUploadErrorData> result;
            protected static ObligationUpload obligationUpload;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetSchemeObligationUpload, List<SchemeObligationUploadErrorData>>>();

                return setup;
            }
        }
    }
}
