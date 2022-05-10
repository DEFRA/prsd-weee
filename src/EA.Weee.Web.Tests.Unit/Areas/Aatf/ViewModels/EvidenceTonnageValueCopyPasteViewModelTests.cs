namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using System.ComponentModel;
    using Xunit;

    public class EvidenceTonnageValueCopyPasteViewModelTests
    {
        [Fact]
        public void ReceievedPastedValues_IsDecoratedWith_DisplayAttribute()
        {
            typeof(EvidenceTonnageValueCopyPasteViewModel).GetProperty("ReceievedPastedValues").Should()
                .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals("Total received (tonnes)"));
        }

        [Fact]
        public void ReusedPastedValues_IsDecoratedWith_DisplayAttribute()
        {
            typeof(EvidenceTonnageValueCopyPasteViewModel).GetProperty("ReusedPastedValues").Should()
                .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals("Reused as whole appliances (tonnes)"));
        }
    }
}
