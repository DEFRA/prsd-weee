namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Web.Mvc;
    using FluentAssertions;
    using Web.Areas.Admin.Controllers;
    using Xunit;

    public class EvidenceReportsControllerTests
    {
        [Fact]
        public void EvidenceReportsController_ShouldDeriveFromReportsBaseController()
        {
            typeof(EvidenceReportsController).Should().BeDerivedFrom<ReportsBaseController>();
        }

        [Fact]
        public void EvidenceNoteReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(EvidenceReportsController).GetMethod("EvidenceNoteReport").Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }
    }
}
