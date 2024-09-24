namespace EA.Weee.Web.Tests.Unit.Service
{
    using EA.Weee.Web.Services;
    using FluentAssertions;
    using System;
    using Xunit;

    public class PaymentReferenceGeneratorTests
    {
        private readonly PaymentReferenceGenerator generator = new PaymentReferenceGenerator();

        [Fact]
        public void GeneratePaymentReference_ShouldReturnCorrectLength()
        {
            var reference = generator.GeneratePaymentReference(20);
            reference.Should().HaveLength(20);
        }

        [Fact]
        public void GeneratePaymentReference_ShouldStartWithWEEEAndYear()
        {
            var reference = generator.GeneratePaymentReference();
            reference.Should().StartWith($"WEEE{DateTime.UtcNow.Year}");
        }

        [Theory]
        [InlineData(9)]
        [InlineData(256)]
        public void GeneratePaymentReference_ShouldThrowArgumentException_WhenLengthIsInvalid(int length)
        {
            Action act = () => generator.GeneratePaymentReference(length);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Length must be between 10 and 255");
        }

        [Fact]
        public void GeneratePaymentReference_ShouldGenerateUniqueReferences()
        {
            var reference1 = generator.GeneratePaymentReference();
            var reference2 = generator.GeneratePaymentReference();
            reference1.Should().NotBe(reference2);
        }

        [Fact]
        public void GeneratePaymentReferenceWithSeparators_ShouldReturnCorrectLength()
        {
            var reference = generator.GeneratePaymentReferenceWithSeparators(20);
            reference.Should().HaveLength(20);
        }

        [Fact]
        public void GeneratePaymentReferenceWithSeparators_ShouldContainCorrectSeparators()
        {
            var reference = generator.GeneratePaymentReferenceWithSeparators(20);
            reference.Should().MatchRegex(@"^WEEE-\d{4}-\d{6}-[A-Z0-9]{3}$");
        }

        [Theory]
        [InlineData(13)]
        [InlineData(256)]
        public void GeneratePaymentReferenceWithSeparators_ShouldThrowArgumentException_WhenLengthIsInvalid(int length)
        {
            Action act = () => generator.GeneratePaymentReferenceWithSeparators(length);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Length must be between 14 and 255");
        }

        [Fact]
        public void GeneratePaymentReferenceWithSeparators_ShouldHaveCorrectFormat()
        {
            var reference = generator.GeneratePaymentReferenceWithSeparators(20);
            reference.Should().MatchRegex(@"^WEEE-\d{4}-\d{6}-[A-Z0-9]{3}$");
        }

        [Fact]
        public void GeneratePaymentReferenceWithSeparators_ShouldGenerateUniqueReferences()
        {
            var reference1 = generator.GeneratePaymentReferenceWithSeparators(20);
            var reference2 = generator.GeneratePaymentReferenceWithSeparators(20);
            reference1.Should().NotBe(reference2);
        }
    }
}