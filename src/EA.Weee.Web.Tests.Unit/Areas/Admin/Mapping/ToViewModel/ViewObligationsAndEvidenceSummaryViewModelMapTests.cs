namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.Admin.Obligation;
    using Core.DataReturns;
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using EA.Weee.Web.Areas.Shared.ToViewModels;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewObligationsAndEvidenceSummaryViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ViewObligationsAndEvidenceSummaryViewModelMap mapper;
        private readonly ITonnageUtilities tonnageUtilities;

        public ViewObligationsAndEvidenceSummaryViewModelMapTests()
        {
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            mapper = new ViewObligationsAndEvidenceSummaryViewModelMap(tonnageUtilities);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => mapper.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_Constructor_ShouldBeDerivedFromObligationEvidenceSummaryDataToViewModel()
        {
            //arrange
            typeof(ViewObligationsAndEvidenceSummaryViewModelMap).Should()
                .BeDerivedFrom<ObligationEvidenceSummaryDataToViewModel<ViewObligationsAndEvidenceSummaryViewModel, ViewObligationsAndEvidenceSummaryViewModelMapTransfer>>();
        }

        [Fact]
        public void Map_Constructor_ShouldImplementIMap()
        {
            //arrange
            typeof(ViewObligationsAndEvidenceSummaryViewModelMap).Should()
                .Implement<IMap<ViewObligationsAndEvidenceSummaryViewModelMapTransfer, ViewObligationsAndEvidenceSummaryViewModel>>();
        }

        [Fact]
        public void Map_GivenSchemeData_SchemeListShouldBeCreated()
        {
            //arrange
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, null, new List<int>(), schemeData);

            //act
            var result = mapper.Map(mapObject);

            //assert
            foreach (var data in schemeData)
            {
                result.SchemeList.Should().ContainEquivalentOf(new OrganisationSchemeData()
                {
                    DisplayName = data.SchemeName,
                    Id = data.Id
                });
            }

            result.SchemeList.Should().BeInAscendingOrder(s => s.DisplayName);
        }

        [Fact]
        public void Map_GivenSchemeData_SchemeListShouldOnlyContainUniqueSchemes()
        {
            //arrange
            var schemeId = TestFixture.Create<Guid>();
            var schemeData = new List<SchemeData>()
            {
                TestFixture.Build<SchemeData>().With(s => s.Id, schemeId).Create(),
                TestFixture.Build<SchemeData>().With(s => s.Id, schemeId).Create()
            };
                
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, null, new List<int>(), schemeData);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeList.Should().OnlyHaveUniqueItems(s => s.Id);
        }

        [Fact]
        public void Map_GivenSourceWithComplianceYears_DisplayNoDataMessageShouldBeFalse()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, null, complianceYears, null);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.DisplayNoDataMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithNoComplianceYears_DisplayNoDataMessageShouldBeTrue()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>(0).ToList();
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, null, complianceYears, null);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.DisplayNoDataMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();

            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, null, complianceYears, null);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeId.Should().Be(schemeId);
            result.ComplianceYearList.Should().BeEquivalentTo(complianceYears);
            result.ObligationEvidenceValues.Should().BeEmpty();
            result.Difference210Total.Should().BeNull();
            result.DifferenceTotal.Should().BeNull();
            result.Evidence210Total.Should().BeNull();
            result.EvidenceTotal.Should().BeNull();
            result.Obligation210Total.Should().BeNull();
            result.ObligationTotal.Should().BeNull();
            result.Reuse210Total.Should().BeNull();
            result.ReuseTotal.Should().BeNull();
            result.TransferredIn210Total.Should().BeNull();
            result.TransferredInTotal.Should().BeNull();
            result.TransferredOut210Total.Should().BeNull();
            result.TransferredOutTotal.Should().BeNull();
        }

        [Fact]
        public void Map_GivenSourceWithNullSchemeData_SchemeIdShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();

            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, null, complianceYears, null);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeId.Should().Be(schemeId);
        }

        [Fact]
        public void Map_GivenSourceWithSchemeDataAndSelectedSchemeIdExistsInSchemeData_SchemeIdShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();
            var schemeData = new List<SchemeData>()
            {
                TestFixture.Build<SchemeData>().With(s => s.Id, schemeId).Create()
            };

            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, null, complianceYears, schemeData);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeId.Should().Be(schemeId);
        }

        [Fact]
        public void Map_GivenSourceWithSchemeDataAndSelectedSchemeIdDoesNotExistInSchemeData_SchemeIdShouldBeNull()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();

            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, null, complianceYears, schemeData);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeId.Should().BeNull();
        }

        [Fact]
        public void Map_GivenSourceNullSchemeId_PropertiesShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();

            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, null, complianceYears, null);

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.SchemeId.Should().BeNull();
            result.ComplianceYearList.Should().BeEquivalentTo(complianceYears);
        }

        [Fact]
        public void Map_GivenObligationSummaryData_ObligationValuesShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var obligationTonnageValues = new List<ObligationEvidenceTonnageData>()
            {
                new ObligationEvidenceTonnageData(WeeeCategory.ITAndTelecommsEquipment, 100M, null, 20M, 0, 3.89M, 20M),
                new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, null, 50.534M, null, 40M, null, null),
                new ObligationEvidenceTonnageData(WeeeCategory.MedicalDevices, 0, 0, 0, 0, 0, 0)
            };

            var obligationSummaryData = new ObligationEvidenceSummaryData(obligationTonnageValues);
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, obligationSummaryData, complianceYears, null);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Obligation)).Returns("100.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Evidence)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Reuse)).Returns("20.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).TransferredOut)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).TransferredIn)).Returns("3.890");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Difference)).Returns("20.000");

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).Obligation)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).Evidence)).Returns("50.534");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).Reuse)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).TransferredOut)).Returns("40.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).TransferredIn)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(1).Difference)).Returns("-");

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).Obligation)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).Evidence)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).Reuse)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).TransferredOut)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).TransferredIn)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(2).Difference)).Returns("-");

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.ObligationEvidenceValues.ElementAt(0).CategoryId.Should().Be(WeeeCategory.ITAndTelecommsEquipment.ToInt());
            result.ObligationEvidenceValues.ElementAt(0).Obligation.Should().Be("100.000");
            result.ObligationEvidenceValues.ElementAt(0).Evidence.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(0).Reused.Should().Be("20.000");
            result.ObligationEvidenceValues.ElementAt(0).TransferredOut.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(0).TransferredIn.Should().Be("3.890");
            result.ObligationEvidenceValues.ElementAt(0).Difference.Should().Be("20.000");

            result.ObligationEvidenceValues.ElementAt(1).CategoryId.Should().Be(WeeeCategory.ElectricalAndElectronicTools.ToInt());
            result.ObligationEvidenceValues.ElementAt(1).Obligation.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(1).Evidence.Should().Be("50.534");
            result.ObligationEvidenceValues.ElementAt(1).Reused.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(1).TransferredOut.Should().Be("40.000");
            result.ObligationEvidenceValues.ElementAt(1).TransferredIn.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(1).Difference.Should().Be("-");

            result.ObligationEvidenceValues.ElementAt(2).CategoryId.Should().Be(WeeeCategory.MedicalDevices.ToInt());
            result.ObligationEvidenceValues.ElementAt(2).Obligation.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(2).Evidence.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(2).Reused.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(2).TransferredOut.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(2).TransferredIn.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(2).Difference.Should().Be("-");
        }

        [Fact]
        public void Map_GivenObligationSummaryData_ObligationTotalsShouldBeSet()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();

            var obligationTonnageValues = new List<ObligationEvidenceTonnageData>()
            {
                new ObligationEvidenceTonnageData(WeeeCategory.LargeHouseholdAppliances, 1, 2, 3, 4, 5, 6),
                new ObligationEvidenceTonnageData(WeeeCategory.DisplayEquipment, 7, 8, 9, 10, 11, 12),
                new ObligationEvidenceTonnageData(WeeeCategory.CoolingApplicancesContainingRefrigerants, 13, 14, 15, 16, 17, 18),
                new ObligationEvidenceTonnageData(WeeeCategory.GasDischargeLampsAndLedLightSources, 19, 20, 21, 22, 23, 24),
                new ObligationEvidenceTonnageData(WeeeCategory.PhotovoltaicPanels, 25, 26, 27, 28, 29, 30),
                new ObligationEvidenceTonnageData(WeeeCategory.MedicalDevices, 1, 2, 3, 4, 5, 6),
                new ObligationEvidenceTonnageData(WeeeCategory.ToysLeisureAndSports, 7, 8, 8, 10, 11, 12),
                new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, 13, 14, 15, 16, 17, 18),
            };

            var excludedCategories = new List<WeeeCategory>()
            {
                WeeeCategory.LargeHouseholdAppliances,
                WeeeCategory.DisplayEquipment,
                WeeeCategory.CoolingApplicancesContainingRefrigerants,
                WeeeCategory.GasDischargeLampsAndLedLightSources,
                WeeeCategory.PhotovoltaicPanels
            };

            var obligationSummaryData = new ObligationEvidenceSummaryData(obligationTonnageValues);
            var mapObject =
                new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(null, obligationSummaryData, complianceYears, null);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Obligation))).Returns("86.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Obligation))).Returns("21.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Evidence))).Returns("94.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Evidence))).Returns("24.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Difference))).Returns("126.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Difference))).Returns("36.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Reuse))).Returns("101.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Reuse))).Returns("26.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredIn))).Returns("118.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredIn))).Returns("33.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredOut))).Returns("110.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredOut))).Returns("30.000");

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.Difference210Total.Should().Be("36.000");
            result.DifferenceTotal.Should().Be("126.000");
            result.Evidence210Total.Should().Be("24.000");
            result.EvidenceTotal.Should().Be("94.000");
            result.Obligation210Total.Should().Be("21.000");
            result.ObligationTotal.Should().Be("86.000");
            result.Reuse210Total.Should().Be("26.000");
            result.ReuseTotal.Should().Be("101.000");
            result.TransferredIn210Total.Should().Be("33.000");
            result.TransferredInTotal.Should().Be("118.000");
            result.TransferredOut210Total.Should().Be("30.000");
            result.TransferredOutTotal.Should().Be("110.000");
        }
    }
}
