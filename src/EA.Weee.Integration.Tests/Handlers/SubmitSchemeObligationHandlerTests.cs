namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain;
    using Domain.Error;
    using Domain.Obligation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using RequestHandlers.Mappings;
    using Requests.Admin.Obligations;

    public class SubmitSchemeObligationRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenISubmitSchemeObligationWithNoErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();
                authority = Query.GetEaCompetentAuthority();
                var role = Query.GetAdminRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }
                
                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,19,20,21,22,23,24,25,26,";

                var fileInfo = new FileInfo("file name", Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                obligationUpload.Should().NotBeNull();
                obligationUpload.Data.Should().Be(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data));
                obligationUpload.FileName.Should().Be(request.FileInfo.FileName);
                obligationUpload.UploadedById.Should().Be(UserId.ToString());
                obligationUpload.CompetentAuthorityId.Should().Be(authority.Id);
                obligationUpload.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligationUpload.ObligationUploadErrors.Count.Should().Be(0);
            };
        }

        [Component]
        public class WhenISubmitSchemeObligationWithDataErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();
                authority = Query.GetEaCompetentAuthority();
                var role = Query.GetAdminRole();

                schemes.Add(scheme1);
                schemes.Add(scheme2);
                
                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},Invalid,2,3,4,5,6,7,8,9,10,11,12,13,
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,19,20,22222222222222222222,22,23,24,25,26,";

                var fileInfo = new FileInfo("file name", Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                obligationUpload.Should().NotBeNull();
                obligationUpload.Data.Should().Be(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data));
                obligationUpload.FileName.Should().Be(request.FileInfo.FileName);
                obligationUpload.UploadedById.Should().Be(UserId.ToString());
                obligationUpload.CompetentAuthorityId.Should().Be(authority.Id);
                obligationUpload.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligationUpload.ObligationUploadErrors.Count.Should().Be(2);
                obligationUpload.ObligationUploadErrors.ElementAt(0).SchemeIdentifier.Should()
                    .Be(schemes.ElementAt(0).ApprovalNumber);
                obligationUpload.ObligationUploadErrors.ElementAt(0).SchemeName.Should()
                    .Be(schemes.ElementAt(0).SchemeName);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.Data);
                obligationUpload.ObligationUploadErrors.ElementAt(1).SchemeIdentifier.Should()
                    .Be(schemes.ElementAt(1).ApprovalNumber);
                obligationUpload.ObligationUploadErrors.ElementAt(1).SchemeName.Should()
                    .Be(schemes.ElementAt(1).SchemeName);
                obligationUpload.ObligationUploadErrors.ElementAt(1).ErrorType.Should()
                    .Be(ObligationUploadErrorType.Data);
            };
        }

        [Component]
        public class WhenISubmitSchemeObligationWithFileErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();
                authority = Query.GetEaCompetentAuthority();
                var role = Query.GetAdminRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                //header incorrect
                var csvHeader =
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,19,20,21,22,23,24,25,26,";

                var fileInfo = new FileInfo("file name", Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                obligationUpload.Should().NotBeNull();
                obligationUpload.Data.Should().Be(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data));
                obligationUpload.FileName.Should().Be(request.FileInfo.FileName);
                obligationUpload.UploadedById.Should().Be(UserId.ToString());
                obligationUpload.CompetentAuthorityId.Should().Be(authority.Id);
                obligationUpload.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };
        }

        public class SubmitSchemeObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<SubmitSchemeObligation, Guid> handler;
            protected static SubmitSchemeObligation request;
            protected static ObligationUpload obligationUpload;
            protected static UKCompetentAuthority authority;
            protected static List<Scheme> schemes;
            protected static Guid result;
            protected static Fixture fixture;
            
            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalAdminUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<SubmitSchemeObligation, Guid>>();
                schemes = new List<Scheme>();
                return setup;
            }
        }
    }
}
