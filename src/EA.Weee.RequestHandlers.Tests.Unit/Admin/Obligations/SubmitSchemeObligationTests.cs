namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System;
    using Core.Shared;
    using FluentAssertions;
    using Requests.Admin.Obligations;
    using Xunit;

    public class SubmitSchemeObligationTests
    {
        [Fact]
        public void SubmitSchemeObligation_GivenNullFileInfo_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new SubmitSchemeObligation(null, CompetentAuthority.England));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void SubmitSchemeObligation_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var fileInfo = new FileInfo("file", new byte[1]);
            var authority = CompetentAuthority.England;

            //act
            var result = new SubmitSchemeObligation(fileInfo, authority);

            //assert
            result.FileInfo.Should().Be(fileInfo);
            result.Authority.Should().Be(authority);
        }
    }
}
