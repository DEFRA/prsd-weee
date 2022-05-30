namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Xunit;

    public class UploadObligationsViewModelMapTests
    {
        private readonly UploadObligationsViewModelMap map;

        public UploadObligationsViewModelMapTests()
        {
            map = new UploadObligationsViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
