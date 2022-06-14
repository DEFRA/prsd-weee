namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using AutoFixture;
    using Core.Shared;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.Obligations;
    using Weee.Tests.Core;
    using Xunit;

    public class UploadObligationsViewModelTests : SimpleUnitTestBase
    {
        [Fact]
        public void File_ShouldHaveDisplayAttribute()
        {
            typeof(UploadObligationsViewModel).GetProperty("File").Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Choose file"));
        }

        [Fact]
        public void File_ShouldHaveRequiredAttribute()
        {
            typeof(UploadObligationsViewModel).GetProperty("File").Should()
                .BeDecoratedWith<RequiredAttribute>(d => d.ErrorMessage.Equals("You must select a file before the system can check for errors"));
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        [InlineData(false, true, true, true)]
        [InlineData(true, true, true, true)]
        public void AnyError_GivenErrors_AnyErrorShouldBeSet(bool displayDataError, bool displayFormatError,
            bool displaySelectFileError, bool expected)
        {
            //arrange
            var model = new UploadObligationsViewModel()
            {
                DisplayDataError = displayDataError, DisplayFormatError = displayFormatError,
                DisplaySelectFileError = displaySelectFileError
            };

            //act
            var result = model.AnyError;

            //assert
            result.Should().Be(expected);
        }

        [Fact]
        public void UploadObligationsViewModel_ConstructorWithAuthority_PropertiesShouldBeInitialised()
        {
            //act
            var model = new UploadObligationsViewModel(TestFixture.Create<CompetentAuthority>());

            //assert
            model.SchemeObligations.Should().BeEmpty();
        }

        [Fact]
        public void UploadObligationsViewModel_ConstructorWithoutAuthority_PropertiesShouldBeInitialised()
        {
            //act
            var model = new UploadObligationsViewModel();

            //assert
            model.SchemeObligations.Should().BeEmpty();
        }

        [Fact]
        public void AnyObligation_GivenNoObligations_ShouldReturnFalse()
        {
            //act
            var model = new UploadObligationsViewModel();

            //assert
            model.AnyObligation.Should().BeFalse();
        }

        [Fact]
        public void AnyObligation_GivenObligations_ShouldReturnTrue()
        {
            //act
            var model = new UploadObligationsViewModel();
            model.SchemeObligations.Add(new SchemeObligationViewModel());

            //assert
            model.AnyObligation.Should().BeTrue();
        }
    }
}
