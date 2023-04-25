namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Filters;
    using FluentAssertions;
    using System;
    using System.Web.Mvc;
    using Xunit;

    public class ObligationsBaseControllerTests
    {
        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ObligationsBaseController).Should().BeDerivedFrom<AdminController>();
        }

        //Select Authority
        [Fact]
        public void SelectAuthorityGet_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            typeof(ObligationsBaseController).GetMethod("SelectAuthority").Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void SelectAuthorityPost_ShouldHaveHttpPostAttribute()
        {
            // assert
            typeof(ObligationsBaseController).GetMethod("SelectAuthority", new[]
                {
                    typeof(SelectAuthorityViewModel),
                }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void SelectAuthorityPost_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ObligationsBaseController).GetMethod("SelectAuthority", new[]
                {
                    typeof(SelectAuthorityViewModel),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        //Upload Obligations
        [Fact]
        public void UploadObligationsGet_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            typeof(ObligationsBaseController).GetMethod("UploadObligations", new[]
            {
                typeof(CompetentAuthority), typeof(Guid), typeof(int), typeof(bool)
            }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void UploadObligationsPost_ShouldHaveHttpPostAttribute()
        {
            // assert
            typeof(ObligationsBaseController).GetMethod("UploadObligations", new[]
                {
                    typeof(UploadObligationsViewModel),
                }).Should()
                  .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void UploadObligationsPost_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ObligationsBaseController).GetMethod("UploadObligations", new[]
                {
                    typeof(UploadObligationsViewModel),
                }).Should()
                  .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        //DownloadTemplate
        [Fact]
        public void DownloadTemplateGet_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            typeof(ObligationsBaseController).GetMethod("DownloadTemplate", new[]
            {
                typeof(CompetentAuthority)
            }).Should()
              .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }


        [Fact]
        public void Controller_IsDecoratedWith_ValidatePcsObligationsEnabledAttribute()
        {
            typeof(ObligationsBaseController).Should()
                .BeDecoratedWith<ValidatePcsObligationsEnabledAttribute>(a => a.Match(new ValidatePcsObligationsEnabledAttribute()));
        }
    }
}
