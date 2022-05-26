namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class RecipientWasteStatusFilterViewModelTests
    {
        [Theory]
        [InlineData("ReceivedId", "Recipient")]
        [InlineData("WasteTypeValue", "Type of waste")]
        [InlineData("NoteStatusValue", "Status")]
        public void RecipientWasteStatusFilterViewModel_Properties_ShouldHaveDisplayAttributes(string property, string display)
        {
            typeof(RecipientWasteStatusFilterViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayAttribute>(a => a.Name.Equals(display));
        }
    }
}
