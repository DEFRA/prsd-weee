namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared.CsvReading;
    using Core.Validation;
    using DataAccess.DataAccess;
    using Domain.Error;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Obligations;
    using Weee.Tests.Core.SpecimenBuilders;
    using Xunit;

    public class ObligationUploadValidatorTests
    {
        private readonly ObligationUploadValidator obligationUploadValidator;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly ITonnageValueValidator tonnageValueValidator;
        private readonly Fixture fixture;

        public ObligationUploadValidatorTests()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new StringDecimalObligationUploadGenerator());
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            tonnageValueValidator = A.Fake<ITonnageValueValidator>();

            obligationUploadValidator = new ObligationUploadValidator(schemeDataAccess, tonnageValueValidator);
        }

        [Fact]
        public async Task Validate_GivenObligationCsvUploads_SchemeShouldBeRetrieved()
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>().ToList();

            //act
            await obligationUploadValidator.Validate(uploads);

            //assert
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(A<string>._))
                .MustHaveHappened(uploads.ToList().Count(), Times.Exactly);

            foreach (var obligationCsvUpload in uploads)
            {
                A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(obligationCsvUpload.SchemeIdentifier))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task Validate_GivenObligationCsvUploadsWithSchemeError_ErrorsShouldContainRelevantError()
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(uploads.ElementAt(0).SchemeIdentifier))
                .Returns((Scheme)null);

            A.CallTo(() => tonnageValueValidator.Validate(A<object>._)).Returns(TonnageValidationResult.Success);

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(uploads.ElementAt(0).SchemeIdentifier) &&
                                          r.SchemeName.Equals(uploads.ElementAt(0).SchemeName) &&
                                          r.Category == null &&
                                          r.Description.Equals(
                                              $"Scheme with identifier {uploads.ElementAt(0).SchemeIdentifier} not recognised") &&
                                          r.ErrorType == ObligationUploadErrorType.Scheme);
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploads_EachTonnageValueShouldBeValidated(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>().ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);

            for (var count = 0; count < uploads.Count; count++)
            {
                var value = $"MyValue {count}";
                property.SetValue(uploads.ElementAt(count), value);
            }

            //act
            await obligationUploadValidator.Validate(uploads);

            //assert
            foreach (var obligationCsvUpload in uploads)
            {
                var newValue = property.GetValue(obligationCsvUpload);
                A.CallTo(() => tonnageValueValidator.Validate(newValue)).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploadsWithDecimalPlaceFormatTonnageError_ErrorsShouldContainRelevantError(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);
            var weeeCategory = property.GetCustomAttributes(typeof(WeeeCategoryAttribute), true)[0] as WeeeCategoryAttribute;
            const string errorValue = "ErrorValue";
            property.SetValue(uploads.ElementAt(0), errorValue);
            var elementToError = uploads.ElementAt(0);

            A.CallTo(() => tonnageValueValidator.Validate(A<string>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageValueValidator.Validate(errorValue)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.DecimalPlaceFormat));

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(elementToError.SchemeIdentifier) &&
                                          r.SchemeName.Equals(elementToError.SchemeName) &&
                                          r.Category == weeeCategory.Category &&
                                          r.Description.Equals($"Category {(int)weeeCategory.Category} is wrong") &&
                                          r.ErrorType == ObligationUploadErrorType.Data);
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploadsWithMaximumDigitsError_ErrorsShouldContainRelevantError(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);
            var weeeCategory = property.GetCustomAttributes(typeof(WeeeCategoryAttribute), true)[0] as WeeeCategoryAttribute;
            const string errorValue = "ErrorValue";
            property.SetValue(uploads.ElementAt(0), errorValue);
            var elementToError = uploads.ElementAt(0);

            A.CallTo(() => tonnageValueValidator.Validate(A<string>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageValueValidator.Validate(errorValue)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.MaximumDigits));

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(elementToError.SchemeIdentifier) &&
                                          r.SchemeName.Equals(elementToError.SchemeName) &&
                                          r.Category == weeeCategory.Category &&
                                          r.Description.Equals($"Category {(int)weeeCategory.Category} is too long") &&
                                          r.ErrorType == ObligationUploadErrorType.Data);
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploadsWithNotNumericalError_ErrorsShouldContainRelevantError(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);
            var weeeCategory = property.GetCustomAttributes(typeof(WeeeCategoryAttribute), true)[0] as WeeeCategoryAttribute;
            const string errorValue = "ErrorValue";
            property.SetValue(uploads.ElementAt(0), errorValue);
            var elementToError = uploads.ElementAt(0);

            A.CallTo(() => tonnageValueValidator.Validate(A<string>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageValueValidator.Validate(errorValue)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical));

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(elementToError.SchemeIdentifier) &&
                                          r.SchemeName.Equals(elementToError.SchemeName) &&
                                          r.Category == weeeCategory.Category &&
                                          r.Description.Equals($"Category {(int)weeeCategory.Category} is wrong") &&
                                          r.ErrorType == ObligationUploadErrorType.Data);
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploadsWithLessThanZeroError_ErrorsShouldContainRelevantError(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);
            var weeeCategory = property.GetCustomAttributes(typeof(WeeeCategoryAttribute), true)[0] as WeeeCategoryAttribute;
            const string errorValue = "ErrorValue";
            property.SetValue(uploads.ElementAt(0), errorValue);
            var elementToError = uploads.ElementAt(0);

            A.CallTo(() => tonnageValueValidator.Validate(A<string>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageValueValidator.Validate(errorValue)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.LessThanZero));

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(elementToError.SchemeIdentifier) &&
                                          r.SchemeName.Equals(elementToError.SchemeName) &&
                                          r.Category == weeeCategory.Category &&
                                          r.Description.Equals($"Category {(int)weeeCategory.Category} is a negative value") &&
                                          r.ErrorType == ObligationUploadErrorType.Data);
        }

        [Theory]
        [InlineData("Cat1")]
        [InlineData("Cat2")]
        [InlineData("Cat3")]
        [InlineData("Cat4")]
        [InlineData("Cat5")]
        [InlineData("Cat6")]
        [InlineData("Cat7")]
        [InlineData("Cat8")]
        [InlineData("Cat9")]
        [InlineData("Cat10")]
        [InlineData("Cat11")]
        [InlineData("Cat12")]
        [InlineData("Cat13")]
        [InlineData("Cat14")]
        public async Task Validate_GivenObligationCsvUploadsWithDecimalPlacesError_ErrorsShouldContainRelevantError(string propertyName)
        {
            //arrange
            var uploads = fixture.CreateMany<ObligationCsvUpload>(3).ToList();
            var property = typeof(ObligationCsvUpload).GetProperty(propertyName);
            var weeeCategory = property.GetCustomAttributes(typeof(WeeeCategoryAttribute), true)[0] as WeeeCategoryAttribute;
            const string errorValue = "ErrorValue";
            property.SetValue(uploads.ElementAt(0), errorValue);
            var elementToError = uploads.ElementAt(0);

            A.CallTo(() => tonnageValueValidator.Validate(A<string>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageValueValidator.Validate(errorValue)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.DecimalPlaces));

            //act
            var results = await obligationUploadValidator.Validate(uploads);

            //assert
            results.Count.Should().Be(1);
            results.Should().Contain(r => r.SchemeIdentifier.Equals(elementToError.SchemeIdentifier) &&
                                          r.SchemeName.Equals(elementToError.SchemeName) &&
                                          r.Category == weeeCategory.Category &&
                                          r.Description.Equals($"Category {(int)weeeCategory.Category} exceeds decimal place limit") &&
                                          r.ErrorType == ObligationUploadErrorType.Data);
        }
    }
}
