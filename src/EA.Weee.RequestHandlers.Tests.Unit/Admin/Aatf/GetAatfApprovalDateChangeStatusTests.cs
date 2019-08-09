namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using Xunit;

    public class GetAatfApprovalDateChangeStatusTests
    {
        private readonly GetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;
        private readonly Fixture fixture;

        public GetAatfApprovalDateChangeStatusTests()
        {
            getAatfApprovalDateChangeStatus = new GetAatfApprovalDateChangeStatus();

            fixture = new Fixture();
        }

        [Fact]
        public async Task Validate_GivenApprovalDateHasNotChanged_EmptyFlagsShouldBeReturned()
        {
            var date = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalDate).Returns(date);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, date);

            result.Should().Be(0);
        }
    }
}
