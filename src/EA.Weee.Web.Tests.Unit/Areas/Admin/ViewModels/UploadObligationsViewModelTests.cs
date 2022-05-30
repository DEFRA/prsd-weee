namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Obligations;
    using Xunit;

    public class UploadObligationsViewModelTests
    {
        [Fact]
        public void File_ShouldHaveDisplayAttribute()
        {
            typeof(UploadObligationsViewModel).GetProperty("File").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Choose file"));
        }

        [Fact]
        public void File_ShouldHaveRequiredAttribute()
        {
            typeof(UploadObligationsViewModel).GetProperty("File").Should()
                .BeDecoratedWith<RequiredAttribute>(d => d.ErrorMessage.Equals("You must select a file before the system can check for errors"));
        }
    }
}
