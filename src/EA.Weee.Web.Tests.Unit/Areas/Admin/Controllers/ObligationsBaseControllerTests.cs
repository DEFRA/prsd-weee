﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
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

        [Fact]
        public void Controller_IsDecoratedWith_ValidatePcsObligationsEnabledAttribute()
        {
            typeof(ObligationsBaseController).Should()
                .BeDecoratedWith<ValidatePcsObligationsEnabledAttribute>(a => a.Match(new ValidatePcsObligationsEnabledAttribute()));
        }
    }
}
