namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReuseTonnageCompareValueAttributeTests
    {
        private const string Error = "Reuse Tonnage Error Message {0}";
        private const string AssertErrorWithCategoryString = "Reuse Tonnage Error Message 10 automatic dispensers";
        private const string CategoryIdProperty = "Category";
        private const string ReceiveCompareTonnage = "ReceiveTonnage";
        private const string ReuseCompareTonnage = "ReuseTonnage";
        private const string AvailableReceiveCompareTonnage = "AvailableReceiveTonnage";
        private const string AvailableReuseCompareTonnage = "AvailableReuseTonnage";
        private const WeeeCategory Category = WeeeCategory.AutomaticDispensers;
        private readonly ITonnageValueValidator tonnageTonnageValueValidator;

        private ReuseTonnageCompareValueAttribute attribute;

        public ReuseTonnageCompareValueAttributeTests()
        {
            tonnageTonnageValueValidator = A.Fake<ITonnageValueValidator>();
            A.CallTo(() => tonnageTonnageValueValidator.Validate(A<object>._)).Returns(TonnageValidationResult.Success);
            A.CallTo(() => tonnageTonnageValueValidator.Validate("Test")).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical));
        }

        [Fact]
        public void Validate_GivenReceivedValueIsNotParsableAsDecimal_ShouldBeValid()
        {
            // Arrange
            var received = "Test";
            var reused = 1;
            var availableReused = 1;
            var availableReceived = 1;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(A<string>._)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical));

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenAvailableReceivedValueIsNotParsableAsDecimal_ShouldBeValid()
        {
            // Arrange
            var received = 1;
            var reused = 1;
            var availableReused = "Test";
            var availableReceived = 1;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            A.CallTo(() => tonnageTonnageValueValidator.Validate(A<string>._)).Returns(new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical));

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenReusedValueIsNotParsableAsDecimal_ShouldBeValid()
        {
            // Arrange
            var received = 1;
            var reused = 1;
            var availableReused = "Test";
            var availableReceived = 1;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenAvailableReusedValueIsNotParsableAsDecimal_ShouldBeValid()
        {
            // Arrange
            var received = 1;
            var reused = 1;
            var availableReused = 1;
            var availableReceived = "Test";
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenReceivedValueIsNull_ShouldBeValid()
        {
            // Arrange
            object received = null;
            var reused = 1;
            var availableReused = 1;
            var availableReceived = 1;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Validate_GivenSummedReceived_IsLessThan_SummedReused_ShouldBeInValid()
        {
            // Arrange
            var received = 2;
            var reused = 5;
            var availableReused = 10;
            var availableReceived = 2;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeOfType<ValidationException>().Which.Message.Should().Be(AssertErrorWithCategoryString);
        }

        [Fact]
        public void Validate_GivenSummedReceived_IsSameAs_SummedReused_ShouldBeValid()
        {
            // Arrange
            var received = 5;
            var reused = 5;
            var availableReused = 5;
            var availableReceived = 5;
            var tonnageValueModel = TransferTonnageValueModel(received, availableReceived, reused, availableReused);
            var validationContext = new ValidationContext(tonnageValueModel);

            attribute = new ReuseTonnageCompareValueAttribute(CategoryIdProperty, ReceiveCompareTonnage, AvailableReceiveCompareTonnage, ReuseCompareTonnage, AvailableReuseCompareTonnage, Error)
            {
                TonnageValueValidator = tonnageTonnageValueValidator
            };

            // Act
            var result = Record.Exception(() => attribute.Validate(tonnageValueModel.ReceiveTonnage, validationContext));

            // Assert
            result.Should().BeNull();
        }

        private TestTransferTonnageValue TransferTonnageValueModel(object received, object availableReceived, object reused, object availableResused)
        {
            var tonnageValueModel = new TestTransferTonnageValue()
            {
                Category = Category,
                ReceiveTonnage = received,
                AvailableReceiveTonnage = availableReceived,
                ReuseTonnage = reused,
                AvailableReuseTonnage = availableResused,
            };

            return tonnageValueModel;
        }

        public class TestTransferTonnageValue
        {
            [ReuseTonnageCompareValue(CategoryIdProperty, ReuseTonnageCompareValueAttributeTests.ReceiveCompareTonnage, ReuseTonnageCompareValueAttributeTests.AvailableReceiveCompareTonnage, ReuseTonnageCompareValueAttributeTests.ReuseCompareTonnage, ReuseTonnageCompareValueAttributeTests.AvailableReuseCompareTonnage, Error)]
            public object ReceiveTonnage { get; set; }
            public WeeeCategory Category { get; set; }
            public object ReuseTonnage { get; set; }
            public object AvailableReceiveTonnage { get; set; }
            public object AvailableReuseTonnage { get; set; }
        }
    }
}
