namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using System.Runtime.InteropServices;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Xunit;

    public class ReturnTests
    {
        [Fact]
        public void Return_GivenOperatorIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(null, A.Dummy<Quarter>(), A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_QuarterIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), null, A.Dummy<string>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_CreatedByIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Return_CreatedByIsEmpty_ThrowsArgumentNullException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), value);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_GivenValidParameters_ReturnPropertiesShouldBeSet()
        {
            var aatfOperator = A.Fake<Operator>();
            var quarter = A.Fake<Quarter>();
            var userId = "user";

            var @return = new Return(aatfOperator, quarter, userId);

            SystemTime.Freeze(new DateTime(2019, 05, 2));

            @return.Operator.Should().Be(aatfOperator);
            @return.Quarter.Should().Be(quarter);
            @return.ReturnStatus.Should().Be(ReturnStatus.Created);
            @return.CreatedBy.Should().Be(userId);
            @return.CreatedDate.Should().BeSameDateAs(new DateTime(2019, 05, 2));

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateSubmitted_GivenSubmittedByIsEmpty_ThrowsArgumentNullException(string value)
        {
            var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), "me");

            Action update = () =>
            {
                @return.UpdateSubmitted("me");
            };

            update.Should().Throw<ArgumentNullException>();
        }
    }
}
