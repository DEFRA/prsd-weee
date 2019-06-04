namespace EA.Weee.Web.Tests.Unit.ViewModels.Returns.Mapping.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Xunit;

    public class ReturnViewModelToReturnsItemViewModelTests
    {
        private readonly IMapper genericMapper;
        private readonly IMap<ReturnData, ReturnsListRedirectOptions> returnListRedirectMap;
        private readonly IMap<ReturnData, ReturnViewModel> returnMap;
        private readonly ReturnToReturnsItemViewModelMapper mapper;

        public ReturnViewModelToReturnsItemViewModelTests()
        {
            genericMapper = A.Fake<IMapper>();
            returnListRedirectMap = A.Fake<IMap<ReturnData, ReturnsListRedirectOptions>>();
            returnMap = A.Fake<IMap<ReturnData, ReturnViewModel>>();

            mapper = new ReturnToReturnsItemViewModelMapper(genericMapper, returnListRedirectMap, returnMap);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => { mapper.Map(null); };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_ReturnViewModelShouldBeMapped()
        {
            var returnData = new ReturnData();
            var returnViewModel = new ReturnViewModel();

            A.CallTo(() => returnMap.Map(returnData)).Returns(returnViewModel);

            var result = mapper.Map(returnData);

            result.ReturnViewModel.Should().Be(returnViewModel);
        }

        [Fact]
        public void Map_GivenSource_ReturnDisplayOptionsListShouldBeMapped()
        {
            var returnViewModel = new ReturnData() { ReturnStatus = ReturnStatus.Created };

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenSource_ReturnDisplayOptionsListAndReturned()
        {
            var returnViewModel = new ReturnData() { ReturnStatus = ReturnStatus.Created };
            var returnsListDisplayOptions = new ReturnsListDisplayOptions();

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).Returns(returnsListDisplayOptions);

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).MustHaveHappened(Repeated.Exactly.Once);
            result.ReturnsListDisplayOptions.Should().Be(returnsListDisplayOptions);
        }

        [Fact]
        public void Map_GivenSource_ReturnRedirectOptionsListShouldBeMapped()
        {
            var returnViewModel = new ReturnData() { ReturnStatus = ReturnStatus.Created };

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => returnListRedirectMap.Map(returnViewModel)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenSource_ReturnRedirectOptionsListAndReturned()
        {
            var returnViewModel = new ReturnData() { ReturnStatus = ReturnStatus.Created };
            var returnsListRedirectOptions = new ReturnsListRedirectOptions();

            A.CallTo(() => returnListRedirectMap.Map(returnViewModel)).Returns(returnsListRedirectOptions);

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => returnListRedirectMap.Map(returnViewModel)).MustHaveHappened(Repeated.Exactly.Once);
            result.ReturnsListRedirectOptions.Should().Be(returnsListRedirectOptions);
        }
    }
}
