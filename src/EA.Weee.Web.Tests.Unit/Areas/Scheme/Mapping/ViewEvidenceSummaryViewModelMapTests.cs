namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Areas.Shared.ToViewModels;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewEvidenceSummaryViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ViewEvidenceSummaryViewModelMap mapper;
        private readonly ITonnageUtilities tonnageUtilities;

        public ViewEvidenceSummaryViewModelMapTests()
        {
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            mapper = new ViewEvidenceSummaryViewModelMap(tonnageUtilities);
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
            typeof(ViewEvidenceSummaryViewModelMap).Should()
                .BeDerivedFrom<ObligationEvidenceSummaryDataToViewModel<SummaryEvidenceViewModel, ViewEvidenceSummaryViewModelMapTransfer>>();
        }

        [Fact]
        public void Map_Constructor_ShouldImplementIMap()
        {
            //arrange
            typeof(ViewEvidenceSummaryViewModelMap).Should()
                .Implement<IMap<ViewEvidenceSummaryViewModelMapTransfer, SummaryEvidenceViewModel>>();
        }

        [Fact]
        public void Map_GivenSourceAndNullObligationValues_PropertiesShouldBeSet()
        {
            // arrange
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            var currentDate = DateTime.Now;

            var source = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, null, null, null, 
                currentDate, complianceYear);

            // act
            var result = mapper.Map(source);

            // assert
            result.ActiveDisplayOption.Should().Be(ManageEvidenceNotesDisplayOptions.Summary);
            result.ObligationEvidenceValues.Should().BeEmpty();
            result.DisplayNoDataMessage.Should().BeFalse();
            result.OrganisationId.Should().Be(organisationId);
            result.SchemeInfo.Should().BeNull();
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
        public void Map_GivenSourceAndObligationEvidenceTonnageDataWithNullValues_DisplayNoDataMessageShouldBeTrue()
        {
            // arrange
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            var currentDate = DateTime.Now;

            var obligation1 = new ObligationEvidenceTonnageData(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null, null, null, null, null);
            var obligation2 = new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, null, null, null, null, null, null);
            var obligation3 = new ObligationEvidenceTonnageData(WeeeCategory.PhotovoltaicPanels, null, null, null, null, null, null);

            var obligationValuesList = new List<ObligationEvidenceTonnageData> { obligation1, obligation2, obligation3 };

            var obligationSummary = new ObligationEvidenceSummaryData(obligationValuesList);

            var source = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, obligationSummary, null, null,
                currentDate, complianceYear);

            // act
            var result = mapper.Map(source);

            // assert
            result.DisplayNoDataMessage.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceAndObligationEvidenceTonnageDataWithValues_DisplayNoDataMessageShouldBeFalse()
        {
            // arrange
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            var currentDate = DateTime.Now;

            var obligation1 = new ObligationEvidenceTonnageData(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, 0, null, null, null, null);
            var obligation2 = new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, null, null, null, null, null, 11);
            var obligation3 = new ObligationEvidenceTonnageData(WeeeCategory.PhotovoltaicPanels, null, null, null, null, null, null);

            var obligationValuesList = new List<ObligationEvidenceTonnageData> { obligation1, obligation2, obligation3 };

            var obligationSummary = new ObligationEvidenceSummaryData(obligationValuesList);

            var source = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, obligationSummary, null, null,
                currentDate, complianceYear);

            // act
            var result = mapper.Map(source);

            // assert
            result.DisplayNoDataMessage.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithCurrentDate_ComplianceYearListShouldBeMapped()
        {
            //arrange
            var currentDate = new DateTime(2019, 1, 1);
            var complianceYear = DateTime.Now.Year;
            var organisationId = TestFixture.Create<Guid>();

            var source = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, null, null, null,
             currentDate, complianceYear);

            //act
            var model = mapper.Map(source);

            // assert 
            model.ManageEvidenceNoteViewModel.ComplianceYearList.Should().BeEquivalentTo(new List<int>() { 2019, 2018, 2017 });
        }

        [Fact]
        public void Map_GivenSourceWithSelectedComplianceYear_SelectedComplianceYearShouldBeMapped()
        {
            //arrange
            var currentDate = DateTime.Now;
            var complianceYear = DateTime.Now.Year;
            var organisationId = TestFixture.Create<Guid>();

            var source = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, null, null, null,
             currentDate, complianceYear);

            //act
            var model = mapper.Map(source);

            // assert 
            model.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public void Map_GivenObligationSummaryData_ObligationEvidenceValuesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var currentDate = DateTime.Now;
            var complianceYear = DateTime.Now.Year;

            var obligationTonnageValues = new List<ObligationEvidenceTonnageData>()
            {
                new ObligationEvidenceTonnageData(WeeeCategory.ITAndTelecommsEquipment, 100M, null, 5M, 0, 3.89M, 18M),
                new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, null, 50.534M, null, 40M, null, null),
                new ObligationEvidenceTonnageData(WeeeCategory.MedicalDevices, 0, 0, 0, 0, 0, 0),
                new ObligationEvidenceTonnageData(WeeeCategory.ToysLeisureAndSports, 0, 1M, 0, 23M, 199M, null)
            };

            var obligationSummaryData = new ObligationEvidenceSummaryData(obligationTonnageValues);
            var mapObject =
                new ViewEvidenceSummaryViewModelMapTransfer(organisationId, obligationSummaryData, null, null,
             currentDate, complianceYear);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Obligation)).Returns("100.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Evidence)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Reuse)).Returns("5.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).TransferredOut)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).TransferredIn)).Returns("3.890");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(0).Difference)).Returns("18.000");

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

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).Obligation)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).Evidence)).Returns("1.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).Reuse)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).TransferredOut)).Returns("199.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).TransferredIn)).Returns("23.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.ElementAt(3).Difference)).Returns("-");

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.ObligationEvidenceValues.ElementAt(0).CategoryId.Should().Be(WeeeCategory.ITAndTelecommsEquipment.ToInt());
            result.ObligationEvidenceValues.ElementAt(0).Obligation.Should().Be("100.000");
            result.ObligationEvidenceValues.ElementAt(0).Evidence.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(0).Reused.Should().Be("5.000");
            result.ObligationEvidenceValues.ElementAt(0).TransferredOut.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(0).TransferredIn.Should().Be("3.890");
            result.ObligationEvidenceValues.ElementAt(0).Difference.Should().Be("18.000");

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

            result.ObligationEvidenceValues.ElementAt(3).CategoryId.Should().Be(WeeeCategory.ToysLeisureAndSports.ToInt());
            result.ObligationEvidenceValues.ElementAt(3).Obligation.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(3).Evidence.Should().Be("1.000");
            result.ObligationEvidenceValues.ElementAt(3).Reused.Should().Be("-");
            result.ObligationEvidenceValues.ElementAt(3).TransferredOut.Should().Be("199.000");
            result.ObligationEvidenceValues.ElementAt(3).TransferredIn.Should().Be("23.000");
            result.ObligationEvidenceValues.ElementAt(3).Difference.Should().Be("-");
        }

        [Fact]
        public void Map_GivenObligationSummaryData_ObligationEvidenceTotalsShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var currentDate = DateTime.Now;
            var complianceYear = DateTime.Now.Year;

            var obligationTonnageValues = new List<ObligationEvidenceTonnageData>()
            {
                new ObligationEvidenceTonnageData(WeeeCategory.LargeHouseholdAppliances, 1, 2, 3, 4, 5, 6),  // ex
                new ObligationEvidenceTonnageData(WeeeCategory.DisplayEquipment, 7, 8, 9, 10, 11, 12),     // ex
                new ObligationEvidenceTonnageData(WeeeCategory.CoolingApplicancesContainingRefrigerants, 13, 14, 15, 16, 17, 18), // ex
                new ObligationEvidenceTonnageData(WeeeCategory.GasDischargeLampsAndLedLightSources, 19, 20, 21, 22, 23, 24),   //ex
                new ObligationEvidenceTonnageData(WeeeCategory.PhotovoltaicPanels, 25, 26, 27, 28, 29, 30),   //ex
                new ObligationEvidenceTonnageData(WeeeCategory.MedicalDevices, 1, 2, 3, 4, 5, 6),
                new ObligationEvidenceTonnageData(WeeeCategory.ToysLeisureAndSports, 7, 8, 8, 10, 11, 12),
                new ObligationEvidenceTonnageData(WeeeCategory.ElectricalAndElectronicTools, 13, 14, 15, 16, 17, 18),
                new ObligationEvidenceTonnageData(WeeeCategory.ToysLeisureAndSports, 7, 7, 7, 19, 7, 2)
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
                new ViewEvidenceSummaryViewModelMapTransfer(organisationId, obligationSummaryData, null, null,
             currentDate, complianceYear);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Obligation))).Returns("93.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Obligation))).Returns("28.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Evidence))).Returns("101.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Evidence))).Returns("31.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Difference))).Returns("133.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Difference))).Returns("43.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.Reuse))).Returns("108.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.Reuse))).Returns("33.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredIn))).Returns("125.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredIn))).Returns("40.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Sum(x => x.TransferredOut))).Returns("129.000");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(obligationSummaryData.ObligationEvidenceValues.Where(x => !excludedCategories.Contains(x.CategoryId))
                    .Sum(x => x.TransferredOut))).Returns("49.000");

            //act
            var result = mapper.Map(mapObject);

            //assert
            result.Obligation210Total.Should().Be("28.000");
            result.ObligationTotal.Should().Be("93.000");
            result.Difference210Total.Should().Be("43.000");
            result.DifferenceTotal.Should().Be("133.000");
            result.Evidence210Total.Should().Be("31.000");
            result.EvidenceTotal.Should().Be("101.000");
            result.Reuse210Total.Should().Be("33.000");
            result.ReuseTotal.Should().Be("108.000");
            result.TransferredIn210Total.Should().Be("40.000");
            result.TransferredInTotal.Should().Be("125.000");
            result.TransferredOut210Total.Should().Be("49.000");
            result.TransferredOutTotal.Should().Be("129.000");
        }
    }
}
