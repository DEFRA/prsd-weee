namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.ComponentModel;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TransferEvidenceTonnageViewModelTests
    {
        [Fact]
        public void TransferEvidenceTonnageViewModel_ShouldBeDerivedFrom_TransferEvidenceViewModelBase()
        {
            typeof(TransferEvidenceTonnageViewModel).Should().BeDerivedFrom<TransferEvidenceViewModelBase>();
        }

        [Fact]
        public void TransferEvidenceTonnageViewModel_ShouldInitialise_TransferCategoryValues()
        {
            //act
            var model = new TransferEvidenceTonnageViewModel();

            //assert
            model.TransferCategoryValues.Should().BeEmpty();
        }

        [Fact]
        public void TransferAllTonnageProperty_ShouldHaveDisplayNameAttribute()
        {
            typeof(TransferEvidenceTonnageViewModel).GetProperty("TransferAllTonnage").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Transfer all available tonnage from all notes that you have selected"));
        }
    }
}
