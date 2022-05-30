namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using AutoFixture;
    using Core.Shared;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Xunit;

    public class UploadObligationsViewModelMapTests
    {
        private readonly UploadObligationsViewModelMap map;
        private readonly Fixture fixture;

        public UploadObligationsViewModelMapTests()
        {
            fixture = new Fixture();

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

        [Fact]
        public void Map_GivenAuthority_AuthorityShouldBeSet()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();

            //act
            var model = map.Map(new UploadObligationsViewModelMapTransfer() { CompetentAuthority = authority });

            //assert
            model.Authority.Should().Be(authority);
        }
    }
}
