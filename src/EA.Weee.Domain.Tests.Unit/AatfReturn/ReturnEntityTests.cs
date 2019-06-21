namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnEntityTests
    {
        private readonly ReturnEntityBaseTest returnEntity;

        public ReturnEntityTests()
        {
            returnEntity = new ReturnEntityBaseTest();
        }

        [Fact]
        public void ReturnEntityBaseTests_ShouldInheritFromReturnEntity()
        {
            typeof(ReturnEntityBaseTest).BaseType.Name.Should().Be(typeof(ReturnEntity).Name);
        }

        [Fact]
        public void UpdateReturn_GivenReturnIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                returnEntity.UpdateReturn(null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateReturn_GivenReturn_ReturnShouldBeUpdated()
        {
            var newReturn = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), "me", A.Dummy<FacilityType>());

            returnEntity.UpdateReturn(newReturn);

            returnEntity.Return.Should().Be(newReturn);
        }
    }
}
