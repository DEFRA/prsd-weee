﻿namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class EditEeeDataViewModelMapTests : SimpleUnitTestBase
    {
        private readonly EditEeeDataViewModelMap map = new EditEeeDataViewModelMap();

        [Fact]
        public void Map_ShouldMapBasicProperties_Correctly()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            var submissionData = source.SmallProducerSubmissionData;

            // Act
            var result = map.Map(source);

            // Assert
            result.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            result.DirectRegistrantId.Should().Be(submissionData.DirectRegistrantId);
            result.SellingTechnique.Should().BeEquivalentTo(SellingTechniqueViewModel.FromSellingTechniqueType(submissionData.CurrentSubmission.SellingTechnique));
            result.HasAuthorisedRepresentitive.Should().Be(submissionData.HasAuthorisedRepresentitive);
        }

        [Fact]
        public void Map_ShouldMapTonnageData_Correctly()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            source.SmallProducerSubmissionData.CurrentSubmission.TonnageData = new List<Eee>
            {
                new Eee(10.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
                new Eee(20.75m, WeeeCategory.DisplayEquipment, ObligationType.B2B)
            };

            // Act
            var result = map.Map(source);

            // Assert
            result.CategoryValues.Should().Contain(cv =>
                cv.CategoryId == (int)WeeeCategory.ConsumerEquipment &&
                cv.HouseHold == "10.500" &&
                cv.NonHouseHold == null);

            result.CategoryValues.Should().Contain(cv =>
                cv.CategoryId == (int)WeeeCategory.DisplayEquipment &&
                cv.HouseHold == null &&
                cv.NonHouseHold == "20.750");
        }

        [Fact]
        public void Map_ShouldHandleEmptyTonnageData_Correctly()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            source.SmallProducerSubmissionData.CurrentSubmission.TonnageData = new List<Eee>();

            // Act
            var result = map.Map(source);

            // Assert
            result.CategoryValues.Should().NotBeNull();
            result.CategoryValues.Should().AllSatisfy(cv =>
            {
                cv.HouseHold.Should().BeNull();
                cv.NonHouseHold.Should().BeNull();
            });
        }

        [Fact]
        public void Map_ShouldHandleMultipleEntriesForSameCategory_ByUsingLastValue()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            source.SmallProducerSubmissionData.CurrentSubmission.TonnageData = new List<Eee>
            {
                new Eee(10.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
                new Eee(5.25m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
                new Eee(20.75m, WeeeCategory.ConsumerEquipment, ObligationType.B2B)
            };

            // Act
            var result = map.Map(source);

            // Assert
            var consumerEquipmentCategory = result.CategoryValues.Single(cv => cv.CategoryId == (int)WeeeCategory.ConsumerEquipment);
            consumerEquipmentCategory.HouseHold.Should().Be("5.250");
            consumerEquipmentCategory.NonHouseHold.Should().Be("20.750");
        }

        [Fact]
        public void Map_ShouldPopulateAllCategories()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            source.SmallProducerSubmissionData.CurrentSubmission.TonnageData = new List<Eee>();

            // Act
            var result = map.Map(source);

            // Assert
            result.CategoryValues.Should().HaveCount(14); 
            result.CategoryValues.Select(cv => cv.CategoryId).Should().BeEquivalentTo(Enumerable.Range(1, 14));
        }

        [Fact]
        public void Map_ShouldSetCategoryDisplayCorrectly()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();
            source.Year = null;
            source.SmallProducerSubmissionData.CurrentSubmission.TonnageData = new List<Eee>
            {
                new Eee(10.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C)
            };

            // Act
            var result = map.Map(source);

            // Assert
            var consumerEquipmentCategory = result.CategoryValues.Single(cv => cv.CategoryId == (int)WeeeCategory.ConsumerEquipment);
            consumerEquipmentCategory.CategoryDisplay.Should().Be(WeeeCategory.ConsumerEquipment.ToDisplayString());
        }

        [Fact]
        public void Map_ShouldMapFromSubmissionsHistory()
        {
            // Arrange
            var source = TestFixture.Create<SmallProducerSubmissionMapperData>();

            var historyData = TestFixture.Create<SmallProducerSubmissionHistoryData>();
            source.Year = 2024;

            source.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>
            { 
                { source.Year.Value, historyData } 
            };

            var submissionData = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

            // Act
            var result = map.Map(source);

            // Assert
            result.OrganisationId.Should().Be(source.SmallProducerSubmissionData.OrganisationData.Id);
            result.DirectRegistrantId.Should().Be(source.SmallProducerSubmissionData.DirectRegistrantId);
            result.SellingTechnique.Should().BeEquivalentTo(SellingTechniqueViewModel.FromSellingTechniqueType(submissionData.SellingTechnique));
            result.HasAuthorisedRepresentitive.Should().Be(source.SmallProducerSubmissionData.HasAuthorisedRepresentitive);
        }
    }
}