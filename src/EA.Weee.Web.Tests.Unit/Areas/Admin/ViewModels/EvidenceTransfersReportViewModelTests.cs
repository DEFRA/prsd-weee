namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.EvidenceReports;
    using Xunit;

    public class EvidenceTransfersReportViewModelTests
    {
        [Fact]
        public void EvidenceTransfersReportViewModel_SelectedYear_ShouldHaveDisplayNameAttribute()
        {
            typeof(EvidenceReportViewModel).GetProperty("SelectedYear").Should()
                .BeDecoratedWith<DisplayAttribute>(e => e.Name.Equals("Compliance year"));
        }

        [Fact]
        public void EvidenceTransfersReportViewModel_SelectedYear_ShouldHaveRequiredAttribute()
        {
            typeof(EvidenceReportViewModel).GetProperty("SelectedYear").Should()
                .BeDecoratedWith<RequiredAttribute>(e => e.ErrorMessage.Equals("Select a compliance year"));
        }
    }
}
