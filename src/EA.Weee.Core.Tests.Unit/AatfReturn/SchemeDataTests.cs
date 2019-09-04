namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using AutoFixture;
    using Core.Shared;
    using FluentAssertions;
    using Scheme;
    using Xunit;

    public class SchemeDataTests
    {
        private readonly Fixture fixture;

        public SchemeDataTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void SchemeNameDisplay_GivenEmptySchemeNameIsNull_SchemeNameDisplayShouldBeCorrect()
        {
            var status = fixture.Create<SchemeStatus>();

            var model = new SchemeData()
            {
                SchemeStatus = status
            };

            model.SchemeNameDisplay.Should().Be($"Empty name ({status})");
        }
    }
}
