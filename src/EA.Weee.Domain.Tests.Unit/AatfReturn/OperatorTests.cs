namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using Xunit;
    using System;
    using Domain.Organisation;
    using FluentAssertions;

    public class OperatorTests
    {
        [Fact]
        public void Operator_GivenOrganisationIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Operator(null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Operator_GivenValidParameteras_OperatorPropertiesShouldBeSet()
        {
            var organisation = A.Fake<Organisation>();

            var @operator = new Operator(organisation);

            @operator.Organisation.Should().Be(@operator);
        }
    }
}
