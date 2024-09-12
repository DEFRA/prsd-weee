namespace EA.Weee.Web.Tests.Unit.Areas.Producer.ViewModel
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class EditEeeDataViewModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCategoryValues()
        {
            // Arrange & Act
            var viewModel = new EditEeeDataViewModel();

            // Assert
            viewModel.CategoryValues.Should().NotBeNull();
            viewModel.CategoryValues.Should().BeOfType<List<ProducerSubmissionCategoryValue>>();
            viewModel.CategoryValues.Should().HaveCount(14); // Based on the WeeeCategory enum
        }

        [Fact]
        public void Constructor_ShouldPopulateCategoryValuesWithAllWeeeCategories()
        {
            // Arrange & Act
            var viewModel = new EditEeeDataViewModel();

            // Assert
            viewModel.CategoryValues.Select(cv => cv.CategoryId).Should().BeEquivalentTo(
                Enum.GetValues(typeof(WeeeCategory)).Cast<int>());
        }

        [Fact]
        public void CategoryValues_ShouldHaveCorrectProperties()
        {
            // Arrange
            var viewModel = new EditEeeDataViewModel();

            // Act & Assert
            foreach (var categoryValue in viewModel.CategoryValues)
            {
                categoryValue.Should().BeOfType<ProducerSubmissionCategoryValue>();
                categoryValue.CategoryId.Should().BeInRange(1, 14);
                categoryValue.CategoryDisplay.Should().NotBeNullOrWhiteSpace();
                categoryValue.HouseHold.Should().BeNull();
                categoryValue.NonHouseHold.Should().BeNull();
            }
        }

        [Fact]
        public void OrganisationId_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var viewModel = new EditEeeDataViewModel();
            var expectedId = Guid.NewGuid();

            // Act
            viewModel.OrganisationId = expectedId;

            // Assert
            viewModel.OrganisationId.Should().Be(expectedId);
        }

        [Fact]
        public void HasAuthorisedRepresentitive_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var viewModel = new EditEeeDataViewModel();

            // Act
            viewModel.HasAuthorisedRepresentitive = true;

            // Assert
            viewModel.HasAuthorisedRepresentitive.Should().BeTrue();
        }

        [Fact]
        public void DirectRegistrantId_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var viewModel = new EditEeeDataViewModel();
            var expectedId = Guid.NewGuid();

            // Act
            viewModel.DirectRegistrantId = expectedId;

            // Assert
            viewModel.DirectRegistrantId.Should().Be(expectedId);
        }

        [Fact]
        public void CategoryValues_ShouldAllowSettingHouseholdAndNonHouseholdValues()
        {
            // Arrange
            var viewModel = new EditEeeDataViewModel();
            var categoryToTest = viewModel.CategoryValues.First(cv => cv.CategoryId == (int)WeeeCategory.LargeHouseholdAppliances);

            // Act
            categoryToTest.HouseHold = "100.5";
            categoryToTest.NonHouseHold = "200.75";

            // Assert
            categoryToTest.HouseHold.Should().Be("100.5");
            categoryToTest.NonHouseHold.Should().Be("200.75");
        }
    }
}