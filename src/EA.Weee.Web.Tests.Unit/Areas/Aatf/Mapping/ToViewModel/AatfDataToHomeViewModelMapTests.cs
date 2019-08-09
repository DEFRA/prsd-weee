namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AatfDataToHomeViewModelMapTests
    {
        private readonly AatfDataToHomeViewModelMap map;

        public AatfDataToHomeViewModelMapTests()
        {
            this.map = new AatfDataToHomeViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            var transfer = new AatfDataToHomeViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), AatfList = A.Fake<List<AatfData>>(), IsAE = false };

            var result = map.Map(transfer);

            result.AatfList.Should().BeEquivalentTo(transfer.AatfList);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.IsAE.Should().Be(transfer.IsAE);
        }
    }
}
