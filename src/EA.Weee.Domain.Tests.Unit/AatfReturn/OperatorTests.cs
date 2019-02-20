namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

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
