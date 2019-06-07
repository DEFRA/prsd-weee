namespace EA.Weee.Web.Tests.Unit.ViewModels.Returns.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.ViewModels.Returns;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Xunit;

    public class ReturnsToReturnsViewModelMapTests
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;
        private readonly ReturnsToReturnsViewModelMap returnsMap;

        public ReturnsToReturnsViewModelMapTests()
        {
            returnItemViewModelMap = A.Fake<IMap<ReturnData, ReturnsItemViewModel>>();
            ordering = A.Fake<IReturnsOrdering>();

            returnsMap = new ReturnsToReturnsViewModelMap(ordering, returnItemViewModelMap);
        }

        [Fact]
        public void Map_GivenMappedReturns_ModelShouldBeReturned()
        {
            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions()
                },
                new ReturnsItemViewModel()
                {
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions()
                }
            };

            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1))
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1))
                }
            };

            var returnsData = new ReturnsData(returnData, null);

            A.CallTo(() => ordering.Order(returnsData.ReturnsList)).Returns(returnsData.ReturnsList.AsEnumerable());
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());
            
            var result = returnsMap.Map(returnsData);

            result.Returns.Should().Contain(returnsItems.ElementAt(0));
            result.Returns.Should().Contain(returnsItems.ElementAt(1));
            result.Returns.Count().Should().Be(2);
        }

        [Fact]
        public void Map_GivenMappedReturnsAreEditableButThereIsAnInProgressReturnInComplianceYearAndQuarter_ReturnedShouldNotBeEditable()
        {
            var returnData = A.CollectionOfFake<ReturnData>(1).ToList();

            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayContinue = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var result = returnsMap.Map(new ReturnsData(returnData, null));

            result.Returns.Count(r => r.ReturnsListDisplayOptions.DisplayEdit).Should().Be(0);
        }

        [Fact]
        public void Map_GivenMappedReturnsAreEditableAndThereAreNoInProgressReturnInComplianceYearAndQuarter_ReturnedShouldBeEditable()
        {
            var returnData = A.CollectionOfFake<ReturnData>(1).ToList();

            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true },
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var result = returnsMap.Map(new ReturnsData(returnData, null));

            result.Returns.ElementAt(0).ReturnsListDisplayOptions.DisplayEdit.Should().BeTrue();
        }

        [Theory]
        [InlineData(2018, QuarterType.Q1)]
        [InlineData(2018, QuarterType.Q2)]
        [InlineData(2018, QuarterType.Q3)]
        [InlineData(2018, QuarterType.Q4)]
        [InlineData(2019, QuarterType.Q2)]
        [InlineData(2019, QuarterType.Q3)]
        [InlineData(2019, QuarterType.Q4)]
        [InlineData(2020, QuarterType.Q1)]
        [InlineData(2020, QuarterType.Q2)]
        [InlineData(2020, QuarterType.Q3)]
        [InlineData(2020, QuarterType.Q4)]
        public void Map_GivenMappedReturnsAreEditableAndThereIsAnInProgressReturnInDifferentComplianceYearAndQuarter_ReturnedShouldBeEditable(int year, QuarterType quarter)
        {
            var returnData = A.CollectionOfFake<ReturnData>(2).ToList();

            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(year, quarter), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayContinue = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var result = returnsMap.Map(new ReturnsData(returnData, null));

            result.Returns.ElementAt(1).ReturnsListDisplayOptions.DisplayEdit.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => returnsMap.Map(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnQuarter_DisplayCreateButtonShouldBeFalse()
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), null);

            var result = returnsMap.Map(returnsData);

            result.DisplayCreateReturn.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenReturnQuarter_DisplayCreateButtonShouldBeTrue()
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), new ReturnQuarter(2019, QuarterType.Q1));

            var result = returnsMap.Map(returnsData);

            result.DisplayCreateReturn.Should().BeTrue();
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public void Map_GivenReturnQuarter_ComplianceReturnPropertiesShouldBeSet(QuarterType quarter)
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), new ReturnQuarter(2019, quarter));

            var result = returnsMap.Map(returnsData);

            result.ComplianceYear.Should().Be(2019);
            result.Quarter.Should().Be(quarter);
        }

        [Fact]
        public void Map_GivenMappedReturnsAreForQuarterAndYearAreEditable_OnlyTheMostRecentRecordForYearAndQuarterShouldBeEditable()
        {
            var returnData = A.CollectionOfFake<ReturnData>(3).ToList();

            var dateNow = DateTime.Now;
            var idToFind = Guid.NewGuid();

            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { CreatedDate = dateNow, Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { CreatedDate = dateNow.AddDays(1), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()) { ReturnId = idToFind },
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { CreatedDate = dateNow.AddDays(-1), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = A.Fake<QuarterWindow>() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var result = returnsMap.Map(new ReturnsData(returnData, null));

            result.Returns.First(r => r.ReturnViewModel.ReturnId.Equals(idToFind)).ReturnsListDisplayOptions.DisplayEdit.Should().BeTrue();
            result.Returns.Count(r => r.ReturnsListDisplayOptions.DisplayEdit.Equals(false)).Should().Be(2);
        }
    }
}
