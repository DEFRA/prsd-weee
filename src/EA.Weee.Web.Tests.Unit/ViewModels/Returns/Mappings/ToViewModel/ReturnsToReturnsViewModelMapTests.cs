namespace EA.Weee.Web.Tests.Unit.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Core.DataReturns;
    using Core.Organisations;
    using EA.Prsd.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Web.ViewModels.Returns;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnsToReturnsViewModelMapTests : SimpleUnitTestBase
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;
        private readonly ReturnsToReturnsViewModelMap returnsMap;
        private readonly Fixture fixture;

        public ReturnsToReturnsViewModelMapTests()
        {
            fixture = new Fixture();
            returnItemViewModelMap = A.Fake<IMap<ReturnData, ReturnsItemViewModel>>();
            ordering = A.Fake<IReturnsOrdering>();

            returnsMap = new ReturnsToReturnsViewModelMap(ordering, returnItemViewModelMap);
        }

        [Fact]
        public void Map_GivenMappedReturnsAreEditableButThereIsAnInProgressReturnInComplianceYearAndQuarter_ReturnedShouldNotBeEditable()
        {
            var returnData = A.CollectionOfFake<ReturnData>(1).ToList();

            var returnsItems = new List<ReturnsItemViewModel>()
            {
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayContinue = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var returnsData = new ReturnsData(returnData, null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(),
                DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

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
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true },
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var returnsData = new ReturnsData(returnData, null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(),
                DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

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
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(year, quarter), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayContinue = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var returnsData = new ReturnsData(returnData, null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(),
                DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.Returns.ElementAt(1).ReturnsListDisplayOptions.DisplayEdit.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => returnsMap.Map(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnsData_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => returnsMap.Map(new ReturnToReturnsViewModelTransfer()));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnQuarter_DisplayCreateButtonShouldBeFalse()
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.DisplayCreateReturn.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenReturnQuarter_DisplayCreateButtonShouldBeTrue()
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), new Quarter(2019, QuarterType.Q1), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.DisplayCreateReturn.Should().BeTrue();
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public void Map_GivenReturnQuarter_ComplianceReturnPropertiesShouldBeSet(QuarterType quarter)
        {
            var returnsData = new ReturnsData(A.CollectionOfFake<ReturnData>(1).ToList(), new Quarter(2019, quarter), A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(), DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

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
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { CreatedDate = dateNow, Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { Id = idToFind, CreatedDate = dateNow.AddDays(1), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                },
                new ReturnsItemViewModel()
                {
                    ReturnViewModel = new ReturnViewModel(new ReturnData() { CreatedDate = dateNow.AddDays(-1), Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow() }, new List<AatfObligatedData>(), A.Fake<OrganisationData>(), new TaskListDisplayOptions()),
                    ReturnsListDisplayOptions = new ReturnsListDisplayOptions() { DisplayEdit = true }
                }
            };

            A.CallTo(() => ordering.Order(A<List<ReturnData>>._)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems.ToArray());

            var returnsData = new ReturnsData(returnData, null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(),
                DateTime.Now);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.Returns.First(r => r.ReturnViewModel.ReturnId.Equals(idToFind)).ReturnsListDisplayOptions.DisplayEdit.Should().BeTrue();
            result.Returns.Count(r => r.ReturnsListDisplayOptions.DisplayEdit.Equals(false)).Should().Be(2);
        }

        [Fact]
        public void Map_GivenNoReturnQuarter_NoOpenWindow_ErrorMessageShouldSayClosed()
        {
            var returnData = A.CollectionOfFake<ReturnData>(3).ToList();

            var returnsData = new ReturnsData(returnData, null, A.Fake<List<Quarter>>(), QuarterWindowTestHelper.GetDefaultQuarterWindow(),
                new DateTime(2019, 3, 17));

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            Assert.Equal("The 2018 compliance period has closed. You can start submitting your 2019 Q1 returns on 1st April.", result.ErrorMessageForNotAllowingCreateReturn);
        }

        [Fact]
        public void Map_GivenNoReturnQuarter_OpenQuarters_ErrorMessageSaysWhenNextQuarterAvailable()
        {
            var openQuarters = new List<Quarter>()
            {
                new Quarter(2019, QuarterType.Q1),
                new Quarter(2019, QuarterType.Q2)
            };

            var nextQuarter = QuarterWindowTestHelper.GetQuarterFourWindow(2019);

            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = openQuarters[1]
                }
            };

            SystemTime.Freeze(new DateTime(2019, 07, 11));

            var returnsData = new ReturnsData(returnData, null, openQuarters, nextQuarter, new DateTime(2019, 3, 1));

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            SystemTime.Unfreeze();

            Assert.Equal($"Returns have been started or submitted for all open quarters. You can start submitting your 2019 Q3 returns on 1st October.", result.ErrorMessageForNotAllowingCreateReturn);
        }

        [Fact]
        public void Map_GivenNoReturnQuarter_OpenQuarters_LatestOpenIsQ4_ErrorMessageSaysWhenNextQuarterAvailableIsQ1()
        {
            var openQuarters = new List<Quarter>()
            {
                new Quarter(2018, QuarterType.Q1),
                new Quarter(2018, QuarterType.Q2),
                new Quarter(2018, QuarterType.Q3),
                new Quarter(2018, QuarterType.Q4)
            };

            var nextQuarter = QuarterWindowTestHelper.GetQuarterOneWindow(2019);

            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = openQuarters[1]
                }
            };

            SystemTime.Freeze(new DateTime(2019, 01, 01));

            var returnsData = new ReturnsData(returnData, null, openQuarters, nextQuarter, new DateTime(2019, 01, 01));

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            SystemTime.Unfreeze();

            Assert.Equal(
                $"Returns have been started or submitted for all open quarters. You can start submitting your 2019 Q1 returns on 1st April.", result.ErrorMessageForNotAllowingCreateReturn);
        }

        [Fact]
        public void Map_GivenNoReturnQuarter_NoReturns_ErrorMessageDisplayedIsNotExpectedToSubmitReturn()
        {
            var returnData = new List<ReturnData>();
            var openQuarters = new List<Quarter>();
            var nextQuarter = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            var returnsData = new ReturnsData(returnData, null, openQuarters, nextQuarter, new DateTime(2019, 3, 1));

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            Assert.Equal("You aren’t expected to submit a return yet. If you think this is wrong, contact your environmental regulator.", result.ErrorMessageForNotAllowingCreateReturn);
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithNullSelectedQuarter_QuarterListPropertyShouldBeSetToQuartersForMostRecentComplianceYear()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q3) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.QuarterList.Count.Should().Be(3);
            result.QuarterList.ElementAt(0).Should().Be("All");
            result.QuarterList.ElementAt(1).Should().Be("Q2");
            result.QuarterList.ElementAt(2).Should().Be("Q3");
            result.SelectedQuarter.Should().Be("All");
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithSelectedComplianceYear_QuarterListPropertyShouldBeSetToQuartersSelectedComplianceYear()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q2) },
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q3) },
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q3) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q3) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q4) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData, SelectedComplianceYear = 2019 });

            result.QuarterList.Count.Should().Be(4);
            result.QuarterList.ElementAt(0).Should().Be("All");
            result.QuarterList.ElementAt(1).Should().Be("Q1");
            result.QuarterList.ElementAt(2).Should().Be("Q2");
            result.QuarterList.ElementAt(3).Should().Be("Q3");
            result.SelectedQuarter.Should().Be("All");
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithSelectedComplianceYear_ReturnsViewModelShouldBeFiltered()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q3) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData, SelectedComplianceYear = 2019 });

            result.Returns.Count(r => r.ReturnViewModel.Year != "2019").Should().Be(0);
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithNullSelectedComplianceYear_ReturnsViewModelShouldBeFilteredToLatestComplianceYear()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q3) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.Returns.Count(r => r.ReturnViewModel.Year != "2020").Should().Be(0);
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithSelectedQuarter_SelectedQuarterPropertyShouldBeSet()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData, SelectedQuarter = "Q1" });

            result.QuarterList.Count.Should().Be(3);
            result.QuarterList.ElementAt(0).Should().Be("All");
            result.QuarterList.ElementAt(1).Should().Be("Q1");
            result.QuarterList.ElementAt(2).Should().Be("Q2");
            result.SelectedQuarter.Should().Be("Q1");
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithSelectedQuarter_ReturnsViewModelShouldBeFiltered()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData, SelectedQuarter = "Q1" });

            result.Returns.Count(r => r.ReturnViewModel.Quarter != "Q1").Should().Be(0);
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithNullSelectedComplianceYear_ComplianceYearListPropertyShouldBeSetReturnsComplianceYears()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2020, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2020, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2021, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2021, fixture.Create<QuarterType>()) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.ComplianceYearList.Count.Should().Be(3);
            result.ComplianceYearList.ElementAt(0).Should().Be(2021);
            result.ComplianceYearList.ElementAt(1).Should().Be(2020);
            result.ComplianceYearList.ElementAt(2).Should().Be(2019);
            result.SelectedComplianceYear.Should().Be(2021);
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithNullSelectedComplianceYear_QuarterListYearShouldBeBasedOffLatestComplianceYear()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, QuarterType.Q1) },
                new ReturnData() { Quarter = new Quarter(2020, QuarterType.Q2) },
                new ReturnData() { Quarter = new Quarter(2021, QuarterType.Q3) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.QuarterList.Count.Should().Be(2);
            result.QuarterList.ElementAt(0).Should().Be("All");
            result.QuarterList.ElementAt(1).Should().Be("Q3");
            result.SelectedQuarter.Should().Be("All");
        }

        [Fact]
        public void Map_GivenSourceReturnDataWithSelectedComplianceYear_SelectedComplianceYearPropertyShouldBeSet()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2020, fixture.Create<QuarterType>()) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData, SelectedComplianceYear = 2019 });

            result.ComplianceYearList.Count.Should().Be(2);
            result.ComplianceYearList.ElementAt(0).Should().Be(2020);
            result.ComplianceYearList.ElementAt(1).Should().Be(2019);
            result.SelectedComplianceYear.Should().Be(2019);
        }

        [Fact]
        public void Map_GivenSourceReturns_NumberOfReturnsPropertyShouldBeMapped()
        {
            var returnsList = new List<ReturnData>()
            {
                new ReturnData() { Quarter = new Quarter(2019, fixture.Create<QuarterType>()) },
                new ReturnData() { Quarter = new Quarter(2020, fixture.Create<QuarterType>()) }
            };

            var returnsData = GetDefaultReturnData(returnsList);

            var result = returnsMap.Map(new ReturnToReturnsViewModelTransfer() { ReturnsData = returnsData });

            result.NumberOfReturns.Should().Be(2);
        }

        private ReturnsData GetDefaultReturnData(List<ReturnData> returnsList)
        {
            var openQuarters = new List<Quarter>();
            var nextQuarter = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            return new ReturnsData(returnsList, null, openQuarters, nextQuarter, fixture.Create<DateTime>());
        }
    }
}
