namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using System.Linq;
    using Core.Helpers;
    using Core.Validation;
    using DataReturns;
    using FluentAssertions;
    using Xunit;

    public class WeeeCategoryEnumDisplayTests
    {
        [Fact]
        public void ToCustomDisplayString_GivenNotITCategory_ShouldDisplayToLower()
        {
            //act & assert
            var categoryValues = Enum.GetValues(typeof(WeeeCategory)).Cast<int>().ToList();

            foreach (var categoryValue in categoryValues)
            {
                if (categoryValue != WeeeCategory.ITAndTelecommsEquipment.ToInt())
                {
                    var categoryEnum = (WeeeCategory)categoryValue;

                    var displayString = categoryEnum.ToCustomDisplayString();

                    displayString.Should().Be(categoryEnum.ToDisplayString().ToLower());
                }
            }
        }

        [Fact]
        public void ToCustomDisplayString_GivenITCategory_ShouldDisplayITCategory()
        {
            //arrange
            
            //act
            var displayString = WeeeCategory.ITAndTelecommsEquipment.ToCustomDisplayString();

            //assert
            displayString.Should().Be(WeeeCategory.ITAndTelecommsEquipment.ToDisplayString());
        }
    }
}
