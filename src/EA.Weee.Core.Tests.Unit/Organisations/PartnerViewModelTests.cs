namespace EA.Weee.Core.Tests.Unit.Organisations
{
    using System.Collections.Generic;
    using EA.Weee.Core.Organisations;
    using FluentAssertions;
    using Xunit;

    public class PartnerViewModelTests
    {
        [Fact]
        public void AllPartnerModels_WhenEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var viewModel = new PartnerViewModel();

            // Act
            var result = viewModel.AllPartnerModels;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void AllPartnerModels_WithOnlyPartnerModels_ShouldReturnCorrectlyOrderedList()
        {
            // Arrange
            var viewModel = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "John", LastName = "Doe" },
                    new AdditionalContactModel { FirstName = "Jane", LastName = "Smith" }
                }
            };

            // Act
            var result = viewModel.AllPartnerModels;

            // Assert
            result.Should().HaveCount(2);
            result[0].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "John", LastName = "Doe", Order = 0 });
            result[1].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Jane", LastName = "Smith", Order = 1 });
        }

        [Fact]
        public void AllPartnerModels_WithOnlyNotRequiredPartnerModels_ShouldReturnCorrectlyOrderedList()
        {
            // Arrange
            var viewModel = new PartnerViewModel
            {
                NotRequiredPartnerModels = new List<NotRequiredPartnerModel>
                {
                    new NotRequiredPartnerModel { FirstName = "Alice", LastName = "Johnson" },
                    new NotRequiredPartnerModel { FirstName = "Bob", LastName = "Williams" }
                }
            };

            // Act
            var result = viewModel.AllPartnerModels;

            // Assert
            result.Should().HaveCount(2);
            result[0].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Alice", LastName = "Johnson", Order = 0 });
            result[1].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Bob", LastName = "Williams", Order = 1 });
        }

        [Fact]
        public void AllPartnerModels_WithBothTypes_ShouldReturnCorrectlyOrderedList()
        {
            // Arrange
            var viewModel = new PartnerViewModel
            {
                PartnerModels = new List<AdditionalContactModel>
                {
                    new AdditionalContactModel { FirstName = "John", LastName = "Doe" },
                    new AdditionalContactModel { FirstName = "Jane", LastName = "Smith" }
                },
                NotRequiredPartnerModels = new List<NotRequiredPartnerModel>
                {
                    new NotRequiredPartnerModel { FirstName = "Alice", LastName = "Johnson" },
                    new NotRequiredPartnerModel { FirstName = "Bob", LastName = "Williams" }
                }
            };

            // Act
            var result = viewModel.AllPartnerModels;

            // Assert
            result.Should().HaveCount(4);
            result[0].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "John", LastName = "Doe", Order = 0 });
            result[1].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Jane", LastName = "Smith", Order = 1 });
            result[2].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Alice", LastName = "Johnson", Order = 2 });
            result[3].Should().BeEquivalentTo(new AdditionalContactModel { FirstName = "Bob", LastName = "Williams", Order = 3 });
        }
    }
}