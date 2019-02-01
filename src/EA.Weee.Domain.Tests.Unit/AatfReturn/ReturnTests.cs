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
                var @return = new Return(null, A.Dummy<Quarter>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_QuarterIsNull_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Operator>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_GivenValidParameteras_ReturnPropertiesShouldBeSet()
        {
            var aatfOperator = A.Fake<Operator>();
            var quater = A.Fake<Quarter>();

            var @return = new Return(aatfOperator, quater);

            @return.Operator.Should().Be(aatfOperator);
            @return.Quarter.Should().Be(quater);
        }
    }
}
