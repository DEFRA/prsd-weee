namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class UploadObligationsViewModelMapTransferTests : SimpleUnitTestBase
    {
        [Fact]
        public void UploadObligationsViewModelMapTransfer_Constructor_ShouldInitialiseProperties()
        {
            //act
            var result = new UploadObligationsViewModelMapTransfer();

            //assert
            result.ObligationData.Should().BeEmpty();
            result.ErrorData.Should().BeNull();
        }
    }
}
