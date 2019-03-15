namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ReturnAndSchemeDataToReceivedPcsViewModelMapTests
    {
        private ReturnAndSchemeDataToReceivedPcsViewModelMap mapper;
        private readonly Scheme mapperTestScheme;
        private readonly AatfData mapperTestAatf;

        public ReturnAndSchemeDataToReceivedPcsViewModelMapTests()
        {
            mapper = new ReturnAndSchemeDataToReceivedPcsViewModelMap(A.Fake<IWeeeCache>(), A.Fake<ITonnageUtilities>());
            mapperTestScheme = new Scheme(Guid.NewGuid(), "Test Scheme");
            mapperTestAatf = new AatfData(Guid.NewGuid(), "Test Aatf", "Aatf approval");
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => mapper.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            const string aatfName = "aatfName";

            var obligatedReceivedData = new List<WeeeObligatedData>
            {
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m),
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m),
            };

            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                ObligatedWeeeReceivedData = obligatedReceivedData
            };

            var schemeDataItems = new List<SchemeData>()
            {
                new SchemeData() { Id = mapperTestScheme.Id, Name = mapperTestScheme.Name }
            };
                        
            var transfer = new ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                AatfName = aatfName,
                ReturnData = returnData,
                SchemeDataItems = schemeDataItems
            };

            var result = mapper.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.AatfName.Should().Be(aatfName);
            result.SchemeList.First().SchemeId.Should().Be(mapperTestScheme.Id);
            //result.SchemeList.First().SchemeName.Should().Be(mapperTestScheme.Name);
            result.SchemeList.First().Tonnages.B2B.Should().Be("2.468");
            result.SchemeList.First().Tonnages.B2C.Should().Be("2.468");
        }
    }
}
