namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class TransferEvidenceNoteDataViewModelTests
    {
        private readonly TransferEvidenceNoteCategoriesViewModel model;

        public TransferEvidenceNoteDataViewModelTests()
        {
            model = new TransferEvidenceNoteCategoriesViewModel();
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_SelectedScheme_ShouldHaveRequiredAttribute()
        {
            var property = "SelectedSchema";
            var description = "Select who you would like to transfer evidence to";

            typeof(TransferEvidenceNoteCategoriesViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<RequiredAttribute>(d => d.ErrorMessage.Equals(description));
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_HasSelectedAtLeastOneCategory_ShouldHaveRequiredAttribute()
        {
            var property = "HasSelectedAtLeastOneCategory";
            var errorMessage = "Select a category you would like to transfer evidence from";

            typeof(TransferEvidenceNoteCategoriesViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<RangeAttribute>(d => d.ErrorMessage.Equals(errorMessage));
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_HasSelectedAtLeastOneCategory_RangeValuesShouldBeTrue()
        {
            var property = "HasSelectedAtLeastOneCategory";
            typeof(TransferEvidenceNoteCategoriesViewModel)
                .GetProperty(property).Should()
                .BeDecoratedWith<RangeAttribute>(e => e.Equals(new RangeAttribute(typeof(bool), "true", "true")));
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_Constructor_ShouldPopulateCategoryValues()
        {
            var categoryValues = new CategoryValues<CategoryBooleanViewModel>();

            for (int i = 0; i < categoryValues.Count; i++)
            {
                model.CategoryBooleanViewModels.ElementAt(i).Should().BeEquivalentTo(categoryValues.ElementAt(i));
            }
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_Constructor_GivenOneCategoryIsSelected_HasSelectedAtLeastOneCategoryShouldBeTrue()
        {
            model.CategoryBooleanViewModels.ElementAt(0).Selected = true;
            model.HasSelectedAtLeastOneCategory.Should().BeTrue();   
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_Constructor_GivenCategoryValuesIsNull_HasSelectedAtLeastOneCategoryShouldBeFalse()
        {
            model.CategoryBooleanViewModels = null;
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
        }

        [Fact]
        public void TransferEvidenceNoteDataViewModel_Constructor_GivenCategoryValuesIsNotNull_HasSelectedAtLeastOneCategoryShouldBeFalse()
        {
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
        }
    }
}
