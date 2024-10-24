﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using Xunit;

    public class AdditionalCompanyDetailsAddressDataMapUnitTests
    {
        private readonly AdditionalCompanyDetailsAddressDataMap map = new AdditionalCompanyDetailsAddressDataMap();

        [Fact]
        public void Map_WithNullSource_ShouldThrowArgumentNullException()
        {
            // Arrange
            ICollection<AdditionalCompanyDetails> source = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => map.Map(source));
        }

        [Fact]
        public void Map_WithEmptySource_ShouldReturnEmptyList()
        {
            // Arrange
            var source = new List<AdditionalCompanyDetails>();

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void Map_WithOrderedSource_ShouldReturnOrderedResult()
        {
            // Arrange
            var source = new List<AdditionalCompanyDetails>
            {
                new AdditionalCompanyDetails { FirstName = "John", LastName = "Doe", Order = 2 },
                new AdditionalCompanyDetails { FirstName = "Jane", LastName = "Smith", Order = 1 },
                new AdditionalCompanyDetails { FirstName = "Bob", LastName = "Johnson", Order = 3 }
            };

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].FirstName.Should().Be("Jane");
            result[1].FirstName.Should().Be("John");
            result[2].FirstName.Should().Be("Bob");
        }

        [Fact]
        public void Map_WithSourceContainingNullValues_ShouldMapCorrectly()
        {
            // Arrange
            var source = new List<AdditionalCompanyDetails>
            {
                new AdditionalCompanyDetails { FirstName = "John", LastName = null },
                new AdditionalCompanyDetails { FirstName = null, LastName = "Smith" },
                new AdditionalCompanyDetails { FirstName = null, LastName = null }
            };

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].FirstName.Should().Be("John");
            result[0].LastName.Should().BeNull();
            result[1].FirstName.Should().BeNull();
            result[1].LastName.Should().Be("Smith");
            result[2].FirstName.Should().BeNull();
            result[2].LastName.Should().BeNull();
        }

        [Fact]
        public void Map_WithMultipleSource_ShouldMapAllItems()
        {
            // Arrange
            var source = new List<AdditionalCompanyDetails>();
            for (var i = 0; i < 5; i++)
            {
                source.Add(new AdditionalCompanyDetails { FirstName = $"FirstName{i}", LastName = $"LastName{i}" });
            }

            // Act
            var result = map.Map(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            for (var i = 0; i < 5; i++)
            {
                result[i].FirstName.Should().Be($"FirstName{i}");
                result[i].LastName.Should().Be($"LastName{i}");
            }
        }
    }
}