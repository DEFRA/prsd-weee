namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Scheme;
    using FluentAssertions;
    using RequestHandlers.Shared;
    using System;
    using System.Threading.Tasks;
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

        [Theory]
        [InlineData("LocalArea", "4DDAD596-153B-436F-B1E2-31FEA920AFB3", "East Anglia (EAN)")]
        [InlineData("LocalArea", "0B04DC2A-DAA7-49A6-9CB3-4B4202CFE122", "Cumbria and Lancashire (CLA)")]
        [InlineData("PanArea", "13D97D30-B94D-491A-BA39-4ABB891917DF", "North")]
        [InlineData("PanArea", "F5767376-6E4A-4C88-AB25-7EC075081BC4", "South East")]
        public async Task FetchLookup_GivenId_ObjectIsRetrieved(string objectType, string id, string name)
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var dataAccess = new CommonDataAccess(database.WeeeContext);

                if (objectType == "LocalArea")
                {
                    var result = await dataAccess.FetchLookup<LocalArea>(Guid.Parse(id));

                    result.Name.Should().Be(name);
                }
                else if (objectType == "PanArea")
                {
                    var result = await dataAccess.FetchLookup<PanArea>(Guid.Parse(id));

                    result.Name.Should().Be(name);
                }
            }
        }

        [Theory]
        [InlineData("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8", "EA")]
        [InlineData("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26", "NIEA")]
        [InlineData("44C2F368-AA66-48F0-BBC9-A0ED34AD0951", "NRW")]
        [InlineData("78F37814-364B-4FAE-BEB5-DB0439CBF177", "SEPA")]
        public async Task GetCompetentAuthorityById_AuthorityShouldBeReturned(Guid id, string abbreviation)
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var dataAccess = new CommonDataAccess(database.WeeeContext);

                var result = await dataAccess.FetchCompetentAuthorityById(id);

                result.Abbreviation.Should().Be(abbreviation);
            }
        }

        [Theory]
        [InlineData(Core.Shared.CompetentAuthority.England, "EA")]
        [InlineData(Core.Shared.CompetentAuthority.Wales, "NRW")]
        [InlineData(Core.Shared.CompetentAuthority.Scotland, "SEPA")]
        [InlineData(Core.Shared.CompetentAuthority.NorthernIreland, "NIEA")]
        public async Task FetchCompetentAuthorityApprovedSchemes_AuthorityShouldBeReturned(Core.Shared.CompetentAuthority authority, string abbreviation)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new CommonDataAccess(database.WeeeContext);

                var result = await dataAccess.FetchCompetentAuthorityApprovedSchemes(authority);

                result.Abbreviation.Should().Be(abbreviation);
            }
        }

        [Fact]
        public async Task FetchCompetentAuthorityApprovedSchemes_OnlyApprovedSchemesReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var authorityId = Guid.Parse("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8");

                var organisation = helper.CreateOrganisation();
                var notApprovedScheme = helper.CreateScheme(organisation, authorityId);

                var dataAccess = new CommonDataAccess(database.WeeeContext);

                await database.Model.SaveChangesAsync();

                var result = await dataAccess.FetchCompetentAuthorityApprovedSchemes(Core.Shared.CompetentAuthority.England);

                result.Schemes.Should().NotContain(x => x.Id == notApprovedScheme.Id);
                result.Schemes.Should().NotContain(x => x.SchemeStatus.Value == SchemeStatus.Rejected.Value);
            }
        }

        [Fact]
        public async Task FetchCompetentAuthorityApprovedSchemes_SchemesReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var authorityId = Guid.Parse("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8");
                var organisation = helper.CreateOrganisation();
                var approvedScheme = helper.CreateScheme(organisation, authorityId);
                approvedScheme.SchemeStatus = SchemeStatus.Approved.Value;

                var dataAccess = new CommonDataAccess(database.WeeeContext);

                await database.Model.SaveChangesAsync();

                var result = await dataAccess.FetchCompetentAuthorityApprovedSchemes(Core.Shared.CompetentAuthority.England);

                result.Schemes.Should().Contain(x => x.SchemeStatus.Value == SchemeStatus.Approved.Value);
                result.Schemes.Should().Contain(x => x.Id == approvedScheme.Id);
            }
        }
    }
}