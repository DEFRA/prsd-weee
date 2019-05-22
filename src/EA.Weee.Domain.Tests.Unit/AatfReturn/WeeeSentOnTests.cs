namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class WeeeSentOnTests
    {
        [Fact]
        public void WeeeSentOn_SiteAddressNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var weeeSentOn = new WeeeSentOn(null, A.Fake<Aatf>(), A.Fake<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeSentOn_AatfNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var weeeSentOn = new WeeeSentOn(A.Fake<AatfAddress>(), null, A.Fake<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeSentOn_ReturnNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var weeeSentOn = new WeeeSentOn(A.Fake<AatfAddress>(), A.Fake<Aatf>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeSentOn_OperatorAddressNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var weeeSentOn = new WeeeSentOn(null, A.Fake<AatfAddress>(), A.Fake<Aatf>(), A.Fake<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_GivenValidParameters_WeeeReceivedPropertiesShouldBeSet()
        {
            var aatf = A.Fake<Aatf>();
            var @return = A.Fake<Return>();
            var siteAddress = A.Fake<AatfAddress>();
            var operatorAddress = A.Fake<AatfAddress>();

            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, aatf, @return);

            weeeSentOn.Aatf.Should().Be(aatf);
            weeeSentOn.Return.Should().Be(@return);
            weeeSentOn.SiteAddress.Should().Be(siteAddress);
        }

        [Fact]
        public void WeeeSentOn_ShouldInheritFromReturnEntity()
        {
            typeof(WeeeSentOn).BaseType.Name.Should().Be(typeof(Domain.AatfReturn.ReturnEntity).Name);
        }
    }
}
