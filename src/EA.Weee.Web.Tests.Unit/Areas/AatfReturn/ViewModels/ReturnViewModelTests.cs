namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ReturnViewModelTests
    {
        [Fact]
        public void AnyAatfSchemes_GivenNoAatfs_FalseShouldBeReturned()
        {
            var model = new ReturnViewModel();

            model.AnyAatfSchemes.Should().BeFalse();
        }

        [Fact]
        public void AnyAatfSchemes_GivenAatfsWithNoSchemes_FalseShouldBeReturned()
        {
            var model = new ReturnViewModel
            {
                AatfsData = new List<AatfObligatedData>()
                {
                    new AatfObligatedData(A.Fake<AatfData>(), new List<AatfSchemeData>())
                }
            };

            model.AnyAatfSchemes.Should().BeFalse();
        }

        [Fact]
        public void AnyAatfSchemes_GivenAatfWithScheme_TrueShouldBeReturned()
        {
            var model = new ReturnViewModel
            {
                AatfsData = new List<AatfObligatedData>()
                {
                    new AatfObligatedData(A.Fake<AatfData>(), new List<AatfSchemeData>()
                    {
                        new AatfSchemeData(A.Fake<SchemeData>(), A.Fake<ObligatedCategoryValue>(), A.Dummy<string>())
                    })
                }
            };

            model.AnyAatfSchemes.Should().BeTrue();
        }
    }
}
