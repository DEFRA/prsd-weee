namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ReturnViewModelToReturnsItemViewModelTests
    {
        private readonly IMapper genericMapper;
        private readonly ReturnViewModelToReturnsItemViewModelMapper mapper;

        public ReturnViewModelToReturnsItemViewModelTests()
        {
            genericMapper = A.Fake<IMapper>();

            mapper = new ReturnViewModelToReturnsItemViewModelMapper(genericMapper);
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
            var returnViewModel = new ReturnViewModel();

            var result = mapper.Map(returnViewModel);

            result.ReturnViewModel.Should().Be(returnViewModel);
        }

        [Fact]
        public void Map_GivenSource_ReturnDisplayOptionsListShouldBeMapped()
        {
            var returnViewModel = new ReturnViewModel() { ReturnData = new ReturnData() { ReturnStatus = ReturnStatus.Created }};

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenSource_ReturnDisplayOptionsListAndReturned()
        {
            var returnViewModel = new ReturnViewModel() { ReturnData = new ReturnData() { ReturnStatus = ReturnStatus.Created } };
            var returnsListDisplayOptions = new ReturnsListDisplayOptions();

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).Returns(returnsListDisplayOptions);

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListDisplayOptions>(returnViewModel.ReturnStatus)).MustHaveHappened(Repeated.Exactly.Once);
            result.ReturnsListDisplayOptions.Should().Be(returnsListDisplayOptions);
        }

        [Fact]
        public void Map_GivenSource_ReturnRedirectOptionsListShouldBeMapped()
        {
            var returnViewModel = new ReturnViewModel() { ReturnData = new ReturnData() { ReturnStatus = ReturnStatus.Created } };

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListRedirectOptions>(returnViewModel)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenSource_ReturnRedirectOptionsListAndReturned()
        {
            var returnViewModel = new ReturnViewModel() { ReturnData = new ReturnData() { ReturnStatus = ReturnStatus.Created } };
            var returnsListRedirectOptions = new ReturnsListRedirectOptions();

            A.CallTo(() => genericMapper.Map<ReturnsListRedirectOptions>(returnViewModel)).Returns(returnsListRedirectOptions);

            var result = mapper.Map(returnViewModel);

            A.CallTo(() => genericMapper.Map<ReturnsListRedirectOptions>(returnViewModel)).MustHaveHappened(Repeated.Exactly.Once);
            result.ReturnsListRedirectOptions.Should().Be(returnsListRedirectOptions);
        }
    }
}
