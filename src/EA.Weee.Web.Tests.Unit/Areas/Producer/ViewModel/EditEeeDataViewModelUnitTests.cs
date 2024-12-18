namespace EA.Weee.Web.Tests.Unit.Areas.Producer.ViewModel
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Validation;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
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

        [Fact]
        public void CategoryValues_ShouldHaveProducerCategoryValuesValidationAttribute()
        {
            // Arrange
            var propertyInfo = typeof(EditEeeDataViewModel).GetProperty(nameof(EditEeeDataViewModel.CategoryValues));

            // Act
            var attribute = propertyInfo.GetCustomAttribute<ProducerCategoryValuesValidationAttribute>();

            // Assert
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void Properties_ShouldNotHaveUnexpectedAttributes()
        {
            // Arrange
            var properties = typeof(EditEeeDataViewModel).GetProperties();

            // Act & Assert
            foreach (var property in properties)
            {
                switch (property.Name)
                {
                    case nameof(EditEeeDataViewModel.CategoryValues):
                        property.GetCustomAttributes().Should().HaveCount(1);
                        property.GetCustomAttribute<ProducerCategoryValuesValidationAttribute>().Should().NotBeNull();
                        break;
                    case nameof(EditEeeDataViewModel.SellingTechnique):
                        property.GetCustomAttributes().Should().HaveCount(1);
                        property.GetCustomAttribute<AtLeastOneCheckedAttribute>().Should().NotBeNull();
                        break;
                    default:
                        property.GetCustomAttributes().Should().BeEmpty();
                        break;
                }
            }
        }

        [Fact]
        public void SellingTechnique_ShouldHaveAtLeastOneCheckedAttribute()
        {
            // Arrange
            var propertyInfo = typeof(EditEeeDataViewModel).GetProperty(nameof(EditEeeDataViewModel.SellingTechnique));

            // Act
            var attribute = propertyInfo.GetCustomAttribute<AtLeastOneCheckedAttribute>();

            // Assert
            attribute.Should().NotBeNull();
            attribute.ErrorMessage.Should().Be("At least one selling technique must be selected");

            // Verify the attribute's constructor parameters
            var validationPropertyName = GetPrivateField(attribute, "validationPropertyName") as string;
            var propertyNamesToCheck = GetPrivateField(attribute, "propertyNamesToCheck") as string[];

            validationPropertyName.Should().Be(nameof(EditEeeDataViewModel.SellingTechnique));
            propertyNamesToCheck.Should().BeEquivalentTo(new[]
            {
                nameof(SellingTechniqueViewModel.IsDirectSelling),
                nameof(SellingTechniqueViewModel.IsIndirectSelling)
            });
        }

        // Helper method to access private fields for testing
        private static object GetPrivateField(object instance, string fieldName)
        {
            return instance.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(instance);
        }
    }
}