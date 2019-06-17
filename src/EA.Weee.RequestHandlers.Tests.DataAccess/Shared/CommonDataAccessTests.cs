namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using RequestHandlers.Scheme;
    using RequestHandlers.Shared;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class CommonDataAccessTests
    {
        [Theory]
        [InlineData("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8", "EA")]
        [InlineData("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26", "NIEA")]
        [InlineData("44C2F368-AA66-48F0-BBC9-A0ED34AD0951", "NRW")]
        [InlineData("78F37814-364B-4FAE-BEB5-DB0439CBF177", "SEPA")]
        public async Task GetEACompetentAuthorityById_AuthorityShouldBeReturned(string id, string abbreviation)
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var dataAccess = new CommonDataAccess(database.WeeeContext);

                var result = await dataAccess.FetchCompetentAuthority(abbreviation);

                result.Abbreviation.Should().Be(abbreviation);
            }
        }
    }
}
