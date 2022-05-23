namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using AutoFixture;
    using EA.Weee.Core.Shared;
    using FakeItEasy;
    using FakeItEasy.Sdk;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Web.Areas.Admin.ViewModels.Obligations;
    using Xunit;

    public class SelectAuthorityViewModelTests
    {
        [Fact]
        public void SelectAuthorityViewModel_ShouldHaveValidOptions()
        {
            SelectAuthorityViewModel model = new SelectAuthorityViewModel();

            model.PossibleValues.Count.Should().Be(3);
            model.PossibleValues.ElementAt(0).Should().Be(CompetentAuthority.England);
            model.PossibleValues.ElementAt(1).Should().Be(CompetentAuthority.Scotland);
            model.PossibleValues.ElementAt(2).Should().Be(CompetentAuthority.NorthernIreland);
        }

        [Fact]
        public void SelectAuthorityViewModel_WithNullSelectedAuthority_ReturnsValidationResult()
        {
            // Arrange
            SelectAuthorityViewModel vm = new SelectAuthorityViewModel();  // SelectedAuthority is null
            ValidationContext ctx = new Fixture().Create<ValidationContext>();

            // Act
            IEnumerable<ValidationResult> results = vm.Validate(ctx);

            // Assert
            Assert.NotNull(results);
            results.Should().HaveCount(1);
            ValidationResult result = results.First();
            Assert.Equal("You must select an option ", result.ErrorMessage);
            Assert.Equal("SelectedAuthority", result.MemberNames.First());
        }
    }
}
