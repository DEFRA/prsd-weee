namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class ViewTransferEvidenceNoteTonnageDataViewModelTests
    {
        [Fact]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(ViewTransferEvidenceNoteTonnageDataViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }
    }
}
