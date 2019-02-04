namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FluentAssertions;

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
            var returnStatus = A.Fake<ReturnStatus>();

            var @return = new Return(aatfOperator, quater, returnStatus);

            @return.Operator.Should().Be(aatfOperator);
            @return.Quarter.Should().Be(quater);
            @return.ReturnStatus.Should().Be(returnStatus);
        }
    }
}
