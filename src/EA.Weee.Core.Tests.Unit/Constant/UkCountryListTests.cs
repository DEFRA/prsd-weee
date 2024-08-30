namespace EA.Weee.Core.Tests.Unit.Constant
{
    using EA.Weee.Core.Constants;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class UkCountryListTests
     {
         [Fact]
         public void ValidCountryIds_ShouldContainFourElements()
         {
             // Act
             int count = UkCountryList.ValidCountryIds.Count;

             // Assert
             count.Should().Be(4);
         }

         [Fact]
         public void ValidCountryIds_ShouldContainExpectedGuids()
         {
             // Arrange
             var expectedGuids = new HashSet<Guid>
             {
                 Guid.Parse("DB83F5AB-E745-49CF-B2CA-23FE391B67A8"),
                 Guid.Parse("4209EE95-0882-42F2-9A5D-355B4D89EF30"),
                 Guid.Parse("184E1785-26B4-4AE4-80D3-AE319B103ACB"),
                 Guid.Parse("7BFB8717-4226-40F3-BC51-B16FDF42550C")
             };

             // Act & Assert
             UkCountryList.ValidCountryIds.Should().BeEquivalentTo(expectedGuids);
         }

         [Theory]
         [InlineData("DB83F5AB-E745-49CF-B2CA-23FE391B67A8")]
         [InlineData("4209EE95-0882-42F2-9A5D-355B4D89EF30")]
         [InlineData("184E1785-26B4-4AE4-80D3-AE319B103ACB")]
         [InlineData("7BFB8717-4226-40F3-BC51-B16FDF42550C")]
         public void ValidCountryIds_ShouldContainSpecificGuid(string guidString)
         {
             // Arrange
             var guid = Guid.Parse(guidString);

             // Act & Assert
             UkCountryList.ValidCountryIds.Should().Contain(guid);
         }

         [Fact]
         public void ValidCountryIds_ShouldNotContainInvalidGuid()
         {
             // Arrange
             var invalidGuid = Guid.NewGuid();

             // Act & Assert
             UkCountryList.ValidCountryIds.Should().NotContain(invalidGuid);
         }
     }
 }