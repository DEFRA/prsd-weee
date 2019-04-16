namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToReturnViewModelMapTests
    {
        private readonly ReturnToReturnViewModelMap map;
        private readonly string nullTonnageDisplay = "-";
        private readonly Guid mapperTestId;
        private readonly int mapperTestYear;
        private readonly Quarter mapperTestQuarter;
        private readonly QuarterWindow mapperTestQuarterWindow;
        private readonly string mapperTestPeriod;
        private readonly List<NonObligatedData> mapperTestNonObligatedData;
        private readonly List<WeeeObligatedData> mapperTestObligatedReceivedData;
        private readonly List<WeeeObligatedData> mapperTestObligatedReusedData;
        private readonly List<WeeeObligatedData> mapperTestObligatedSentOnData;
        private readonly Scheme mapperTestScheme;
        private readonly AatfData mapperTestAatf;
        private readonly List<AatfData> mapperTestAatfList;
        
        public ReturnToReturnViewModelMapTests()
        {
            map = new ReturnToReturnViewModelMap(new TonnageUtilities());
            mapperTestId = new Guid();
            mapperTestYear = 2019;
            mapperTestQuarter = new Quarter(mapperTestYear, QuarterType.Q1);
            mapperTestQuarterWindow = new QuarterWindow(new DateTime(mapperTestYear, 1, 1), new DateTime(mapperTestYear, 3, 31));
            mapperTestPeriod = "Q1 Jan - Mar";
            mapperTestNonObligatedData = new List<NonObligatedData>();
            mapperTestObligatedReceivedData = new List<WeeeObligatedData>();
            mapperTestObligatedReusedData = new List<WeeeObligatedData>();
            mapperTestObligatedSentOnData = new List<WeeeObligatedData>();
            mapperTestScheme = new Scheme(Guid.NewGuid(), "Test Scheme");
            mapperTestAatf = new AatfData(Guid.NewGuid(), "Test Aatf", "Aatf approval");
            mapperTestAatfList = new List<AatfData>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            mapperTestNonObligatedData.Add(new NonObligatedData(0, (decimal)1.234, false, Guid.NewGuid()));
            mapperTestNonObligatedData.Add(new NonObligatedData(0, (decimal)1.234, true, Guid.NewGuid()));

            mapperTestObligatedReceivedData.Add(new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedReusedData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedSentOnData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));

            mapperTestAatfList.Add(mapperTestAatf);

            var schemeDataList = CreateSchemeDataItems();

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData,
                ObligatedWeeeReceivedData = mapperTestObligatedReceivedData,
                ObligatedWeeeReusedData = mapperTestObligatedReusedData,
                ObligatedWeeeSentOnData = mapperTestObligatedSentOnData,
                Aatfs = mapperTestAatfList,
                SchemeDataItems = schemeDataList
            };

            var result = map.Map(returnData);

            result.ReturnId.Should().Be(mapperTestId);
            result.Quarter.Should().Be(mapperTestQuarter.Q.ToString());
            result.Year.Should().Be(mapperTestYear.ToString());
            result.Period.Should().Be(mapperTestPeriod);
            result.NonObligatedTonnageTotal.Should().Be("1.234");
            result.NonObligatedTonnageTotalDcf.Should().Be("1.234");
            result.AatfsData[0].WeeeReceived.B2B.Should().Be("1.234");
            result.AatfsData[0].WeeeReceived.B2C.Should().Be("1.234");
            result.AatfsData[0].WeeeReused.B2B.Should().Be("1.234");
            result.AatfsData[0].WeeeReused.B2C.Should().Be("1.234");
            result.AatfsData[0].WeeeSentOn.B2B.Should().Be("1.234");
            result.AatfsData[0].WeeeSentOn.B2C.Should().Be("1.234");
            result.AatfsData[0].SchemeData[0].Received.B2B.Should().BeEquivalentTo(result.AatfsData[0].WeeeReceived.B2B);
            result.AatfsData[0].SchemeData[0].Received.B2C.Should().BeEquivalentTo(result.AatfsData[0].WeeeReceived.B2C);
        }

        [Fact]
        public void Map_GivenNullObligatedData_ReturnsNullTonnageDisplay()
        {
            mapperTestObligatedReceivedData.Add(new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, null, null));
            mapperTestObligatedReusedData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, null, null));
            mapperTestObligatedSentOnData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, null, null));
            mapperTestAatfList.Add(mapperTestAatf);

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData,
                ObligatedWeeeReceivedData = mapperTestObligatedReceivedData,
                ObligatedWeeeReusedData = mapperTestObligatedReusedData,
                ObligatedWeeeSentOnData = mapperTestObligatedSentOnData,
                Aatfs = mapperTestAatfList
            };

            var result = map.Map(returnData);

            ReturnViewModelMapCommonAsserts(result);
            result.AatfsData[0].WeeeReceived.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReceived.B2C.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReused.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReused.B2C.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeSentOn.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeSentOn.B2C.Should().Be(nullTonnageDisplay);
        }

        [Fact]
        public void Map_GivenNullNonObligatedTonnage_ReturnsNullTonnageDisplay()
        {
            mapperTestNonObligatedData.Add(new NonObligatedData(0, null, false, Guid.NewGuid()));
            mapperTestNonObligatedData.Add(new NonObligatedData(0, null, true, Guid.NewGuid()));

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData
            };

            var result = map.Map(returnData);
            ReturnViewModelMapCommonAsserts(result);
            result.NonObligatedTonnageTotal.Should().Be(nullTonnageDisplay);
            result.NonObligatedTonnageTotalDcf.Should().Be(nullTonnageDisplay);
            result.AatfsData.Count.Should().Be(0);
        }

        [Fact]
        public void Map_GivenNullAatfList_ReturnsNoObligatedData()
        {
            mapperTestObligatedReceivedData.Add(new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedReusedData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedSentOnData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData,
                ObligatedWeeeReceivedData = mapperTestObligatedReceivedData,
                ObligatedWeeeReusedData = mapperTestObligatedReusedData,
                ObligatedWeeeSentOnData = mapperTestObligatedSentOnData,
                Aatfs = mapperTestAatfList
            };

            var result = map.Map(returnData);

            ReturnViewModelMapCommonAsserts(result);
            result.NonObligatedTonnageTotal.Should().Be(nullTonnageDisplay);
            result.NonObligatedTonnageTotalDcf.Should().Be(nullTonnageDisplay);
            result.AatfsData.Count.Should().Be(0);
        }

        [Fact]
        public void Map_GivenAatfInReceivedDataNotPresentInAatfList_ReturnsNullTonnageDisplay()
        {
            mapperTestObligatedReceivedData.Add(new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedReusedData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestObligatedSentOnData.Add(new WeeeObligatedData(Guid.NewGuid(), null, mapperTestAatf, 0, 1.234m, 1.234m));
            mapperTestAatfList.Add(new AatfData(Guid.NewGuid(), "Other New Aatf", "123456789"));

            var returnData = new ReturnData()
            {
                Id = mapperTestId,
                Quarter = mapperTestQuarter,
                QuarterWindow = mapperTestQuarterWindow,
                NonObligatedData = mapperTestNonObligatedData,
                ObligatedWeeeReceivedData = mapperTestObligatedReceivedData,
                ObligatedWeeeReusedData = mapperTestObligatedReusedData,
                ObligatedWeeeSentOnData = mapperTestObligatedSentOnData,
                Aatfs = mapperTestAatfList
            };

            var result = map.Map(returnData);
            
            ReturnViewModelMapCommonAsserts(result);
            result.NonObligatedTonnageTotal.Should().Be(nullTonnageDisplay);
            result.NonObligatedTonnageTotalDcf.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReceived.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReceived.B2C.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReused.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeReused.B2C.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeSentOn.B2B.Should().Be(nullTonnageDisplay);
            result.AatfsData[0].WeeeSentOn.B2C.Should().Be(nullTonnageDisplay);
        }

        private void ReturnViewModelMapCommonAsserts(Web.Areas.AatfReturn.ViewModels.ReturnViewModel result)
        {
            result.ReturnId.Should().Be(mapperTestId);
            result.Quarter.Should().Be(mapperTestQuarter.Q.ToString());
            result.Year.Should().Be(mapperTestYear.ToString());
            result.Period.Should().Be(mapperTestPeriod);
        }

        private List<SchemeData> CreateSchemeDataItems()
        {
            var schemeDataItems = new List<SchemeData>()
            {
                new SchemeData() { Id = mapperTestScheme.Id, SchemeName = mapperTestScheme.Name, ApprovalName = "ABC123" }
            };
            return schemeDataItems;
        }
    }
}