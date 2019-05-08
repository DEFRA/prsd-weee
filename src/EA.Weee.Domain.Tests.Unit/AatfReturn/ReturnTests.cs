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
        public void Return_CreatedByIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), value);
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Return_GivenValidParameters_ReturnPropertiesShouldBeSet()
        {
            var aatfOperator = A.Fake<Operator>();
            var quarter = A.Fake<Quarter>();
            var userId = "user";

            SystemTime.Freeze(new DateTime(2019, 05, 2));
            var @return = new Return(aatfOperator, quarter, userId);
            SystemTime.Unfreeze();

            @return.Operator.Should().Be(aatfOperator);
            @return.Quarter.Should().Be(quarter);
            @return.ReturnStatus.Should().Be(ReturnStatus.Created);
            @return.CreatedById.Should().Be(userId);
            @return.CreatedDate.Date.Should().Be(new DateTime(2019, 05, 2));
            @return.SubmittedDate.Should().BeNull();
            @return.SubmittedById.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        public void UpdateSubmitted_GivenSubmittedByIsEmpty_ThrowsArgumentException(string value)
        {
            var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), "me");

            Action update = () =>
            {
                @return.UpdateSubmitted(value);
            };

            update.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateSubmitted_GivenSubmittedByIsNull_ThrowsArgumentNullException()
        {
            var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), "me");

            Action update = () =>
            {
                @return.UpdateSubmitted(null);
            };

            update.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateSubmitted_GivenSubmittedBy_ReturnSubmittedPropertiesShouldBeSet()
        {
            var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), "me");

            SystemTime.Freeze(new DateTime(2019, 05, 2));
            @return.UpdateSubmitted("me2");
            SystemTime.Unfreeze();

            @return.SubmittedDate.Value.Date.Should().Be(new DateTime(2019, 05, 2));
            @return.SubmittedById.Should().Be("me2");
            @return.ReturnStatus.Should().Be(ReturnStatus.Submitted);
        }

        [Fact]
        public void UpdateSubmitted_GivenReturnIsNotCreatedStatus_InvalidOperationExceptionExpected()
        {
            var @return = new Return(A.Dummy<Operator>(), A.Dummy<Quarter>(), "me") { ReturnStatus = ReturnStatus.Submitted };

            Action update = () =>
            {
                @return.UpdateSubmitted("me2");
            };

            update.Should().Throw<InvalidOperationException>();
        }
    }
}
