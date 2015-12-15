namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class AatfDeliveredAmountTests
    {
        [Fact]
        public void ConstructsAatfDeliveredAmount_WithNullAatfDeliveryLocation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, null, A.Fake<DataReturnVersion>()));
        }

        [Fact]
        public void ConstructsAatfDeliveredAmount_WithNullDataReturnVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AatfDeliveredAmount(ObligationType.B2B, A<WeeeCategory>._, A<decimal>._, A.Fake<AatfDeliveryLocation>(), null));
        }
    }
}
