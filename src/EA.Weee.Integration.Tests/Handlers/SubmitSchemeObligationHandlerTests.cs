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
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14
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
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(0);
            };

            private readonly Cleanup cleanup = LocalCleanup;
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
                {scheme1.ApprovalNumber},{scheme1.SchemeName},Invalid,2,3,4,5,6,7,8,9,10,11,12,13,14
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,190000000000000,20,21,22,23,24,25,26,";

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
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(2);
                obligationUpload.ObligationUploadErrors.Should().Contain(e => e.SchemeIdentifier.Equals(schemes.ElementAt(0).ApprovalNumber)
                        && e.SchemeName.Equals(schemes.ElementAt(0).SchemeName) && 
                        e.ErrorType.Equals(ObligationUploadErrorType.Data));
                obligationUpload.ObligationUploadErrors.Should().Contain(e => e.SchemeIdentifier.Equals(schemes.ElementAt(1).ApprovalNumber)
                                                                              && e.SchemeName.Equals(schemes.ElementAt(1).SchemeName) &&
                                                                              e.ErrorType.Equals(ObligationUploadErrorType.Data));
            };

            private readonly Cleanup cleanup = LocalCleanup;
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
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t),
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14
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
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithSchemeErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                authority = Query.GetEaCompetentAuthority();
                var role = Query.GetAdminRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                //header incorrect
                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t),
                {scheme1.ApprovalNumber}nomatch,{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,";

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
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.Scheme);
            };

            private readonly Cleanup cleanup = LocalCleanup;
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
            protected static void LocalCleanup()
            {
                obligationUpload = null;
                schemes = null;
            }

            protected static void MapStandardProperties()
            {
                obligationUpload.Should().NotBeNull();
                obligationUpload.Data.Should().Be(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data));
                obligationUpload.FileName.Should().Be(request.FileInfo.FileName);
                obligationUpload.UploadedById.Should().Be(UserId.ToString());
                obligationUpload.CompetentAuthorityId.Should().Be(authority.Id);
                obligationUpload.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
            }
        }
    }
}
