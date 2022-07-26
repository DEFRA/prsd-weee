namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using System.ComponentModel;
    using EA.Weee.Web.Areas.Aatf.Attributes;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TransferEvidenceTonnageViewModelTests
    {
        [Fact]
        public void TransferEvidenceTonnageViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(TransferEvidenceTonnageViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }

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
        public void TransferEvidenceTonnageViewModel_TransferCategoryValues_ShouldHaveRequiredTonnageAttribute()
        {
            typeof(TransferEvidenceTonnageViewModel).GetProperty("TransferCategoryValues").Should().BeDecoratedWith<RequiredTonnageAttribute>();
        }

        [Fact]
        public void TransferAllTonnageProperty_ShouldHaveDisplayNameAttribute()
        {
            typeof(TransferEvidenceTonnageViewModel).GetProperty("TransferAllTonnage").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Transfer all available tonnage from all notes that you have selected"));
        }
    }
}
