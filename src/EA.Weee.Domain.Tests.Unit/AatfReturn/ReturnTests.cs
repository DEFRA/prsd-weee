namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnTests
    {
        [Fact]
        public void Return_GivenOperatorIsNull_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(null, A.Dummy<Quarter>(), A.Dummy<ReturnStatus>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_QuarterIsNull_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), null, A.Dummy<ReturnStatus>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_ReturnStatusIsNull_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_GivenValidParameters_ReturnPropertiesShouldBeSet()
        {
            var aatfOperator = A.Fake<Operator>();
            var quater = A.Fake<Quarter>();

            var @return = new Return(aatfOperator, quater, ReturnStatus.Created);

            @return.Operator.Should().Be(aatfOperator);
            @return.Quarter.Should().Be(quater);
            @return.ReturnStatus.Should().Be(ReturnStatus.Created);
        }
    }
}
