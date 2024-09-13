//namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Mapping.ToViewModel
//{
//    using System;
//    using System.Collections.Generic;
//    using EA.Weee.Core.DataReturns;
//    using EA.Weee.Core.DirectRegistrant;
//    using EA.Weee.Core.Helpers;
//    using EA.Weee.Core.Organisations;
//    using EA.Weee.Core.Shared;
//    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
//    using EA.Weee.Web.Areas.Producer.ViewModels;
//    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
//    using FakeItEasy;
//    using FluentAssertions;
//    using Xunit;

//    public class EditEeeDataViewModelMapTests
//    {
//        private readonly EditEeeDataViewModelMap mapper;
//        private readonly ITonnageUtilities fakeTonnageUtilities;

//        public EditEeeDataViewModelMapTests()
//        {
//            fakeTonnageUtilities = A.Fake<ITonnageUtilities>();
//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(A<decimal?>.Ignored))
//                .ReturnsLazily((decimal? t) => t.HasValue ? $"{t.Value:F3}" : "-");

//            mapper = new EditEeeDataViewModelMap(fakeTonnageUtilities);
//        }

//        [Fact]
//        public void Map_NullInput_ReturnsEmptyViewModel()
//        {
//            var result = mapper.Map(null);

//            result.Should().NotBeNull();
//            result.CategoryValues.Should().NotBeNull();
//            result.CategoryValues.Should().HaveCount(14);
//        }

//        [Fact]
//        public void Map_ValidInput_MapsCorrectly()
//        {
//            var input = new SmallProducerSubmissionData
//            {
//                OrganisationData = new OrganisationData { Id = Guid.NewGuid() },
//                DirectRegistrantId = Guid.NewGuid(),
//                HasAuthorisedRepresentitive = false,
//                CurrentSubmission = new SmallProducerSubmissionHistoryData
//                {
//                    TonnageData = new List<Eee>
//                    {
//                        new Eee(10.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
//                        new Eee(5.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2B),
//                        new Eee(7.5m, WeeeCategory.AutomaticDispensers, ObligationType.B2C)
//                    }
//                }
//            };

//            var result = mapper.Map(input);

//            result.Should().NotBeNull();
//            result.OrganisationId.Should().Be(input.OrganisationData.Id);
//            result.DirectRegistrantId.Should().Be(input.DirectRegistrantId);
//            result.HasAuthorisedRepresentitive.Should().BeFalse();
//            result.CategoryValues.Should().HaveCount(14); 
//            result.CategoryValues.Should().Contain(c => c.CategoryId == (int)WeeeCategory.ConsumerEquipment &&
//                                                        c.HouseHold == "10.500" &&
//                                                        c.NonHouseHold == "5.500");
//            result.CategoryValues.Should().Contain(c => c.CategoryId == (int)WeeeCategory.AutomaticDispensers &&
//                                                        c.HouseHold == "7.500" &&
//                                                        c.NonHouseHold == "-");

//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(10.5m)).MustHaveHappened();
//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(5.5m)).MustHaveHappened();
//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(7.5m)).MustHaveHappened();
//        }

//        [Fact]
//        public void Map_MultipleEntriesForSameCategory_SumsTonnages()
//        {
//            var input = new SmallProducerSubmissionData
//            {
//                OrganisationData = new OrganisationData { Id = Guid.NewGuid() },
//                DirectRegistrantId = Guid.NewGuid(),
//                CurrentSubmission = new SmallProducerSubmissionHistoryData
//                {
//                    TonnageData = new List<Eee>
//                    {
//                        new Eee(10.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
//                        new Eee(5.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2C),
//                        new Eee(7.5m, WeeeCategory.ConsumerEquipment, ObligationType.B2B)
//                    }
//                }
//            };

//            var result = mapper.Map(input);

//            result.Should().NotBeNull();
//            result.CategoryValues.Should().HaveCount(14); // Assuming there are 14 WEEE categories
//            result.CategoryValues.Should().Contain(c => c.CategoryId == (int)WeeeCategory.ConsumerEquipment &&
//                                                        c.HouseHold == "16.000" &&
//                                                        c.NonHouseHold == "7.500");

//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(16.0m)).MustHaveHappened();
//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(7.5m)).MustHaveHappened();
//        }

//        [Fact]
//        public void Map_EmptyTonnageData_ReturnsAllCategoriesWithZeroValues()
//        {
//            var input = new SmallProducerSubmissionData
//            {
//                OrganisationData = new OrganisationData { Id = Guid.NewGuid() },
//                DirectRegistrantId = Guid.NewGuid(),
//                CurrentSubmission = new SmallProducerSubmissionHistoryData
//                {
//                    TonnageData = new List<Eee>()
//                }
//            };

//            var result = mapper.Map(input);

//            result.Should().NotBeNull();
//            result.CategoryValues.Should().HaveCount(14); // Assuming there are 14 WEEE categories
//            result.CategoryValues.Should().OnlyContain(c => c.HouseHold == "-" && c.NonHouseHold == "-");

//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(null)).MustHaveHappened(28); // 14 categories * 2 (HouseHold and NonHouseHold)
//        }

//        [Fact]
//        public void Map_NullTonnageData_ReturnsAllCategoriesWithZeroValues()
//        {
//            var input = new SmallProducerSubmissionData
//            {
//                OrganisationData = new OrganisationData { Id = Guid.NewGuid() },
//                DirectRegistrantId = Guid.NewGuid(),
//                CurrentSubmission = new SmallProducerSubmissionHistoryData
//                {
//                    TonnageData = null
//                }
//            };

//            var result = mapper.Map(input);

//            result.Should().NotBeNull();
//            result.CategoryValues.Should().HaveCount(14); // Assuming there are 14 WEEE categories
//            result.CategoryValues.Should().OnlyContain(c => c.HouseHold == "-" && c.NonHouseHold == "-");

//            A.CallTo(() => fakeTonnageUtilities.CheckIfTonnageIsNull(null)).MustHaveHappened(28); // 14 categories * 2 (HouseHold and NonHouseHold)
//        }
//    }
//}