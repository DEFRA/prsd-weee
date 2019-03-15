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

        public ReturnAndSchemeDataToReceivedPcsViewModelMapTests()
        {
            mapper = new ReturnAndSchemeDataToReceivedPcsViewModelMap(A.Fake<IWeeeCache>(), new TonnageUtilities());
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
            var mapperTestScheme = new Scheme(Guid.NewGuid(), "Test Scheme");
            var mapperTestScheme2 = new Scheme(Guid.NewGuid(), "Test Scheme2");
            var mapperTestAatf = new AatfData(aatfId, aatfName, "Aatf approval");

            var obligatedReceivedData = new List<WeeeObligatedData>
            {
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m),
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme, mapperTestAatf, 0, 1.234m, 1.234m),
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme2, mapperTestAatf, 0, 0.234m, 2.234m),
                new WeeeObligatedData(Guid.NewGuid(), mapperTestScheme2, mapperTestAatf, 0, 0.234m, 2.234m)
            };

            var returnData = new ReturnData()
            {
                Id = Guid.NewGuid(),
                ObligatedWeeeReceivedData = obligatedReceivedData
            };

            var schemeDataItems = new List<SchemeData>()
            {
                new SchemeData() { Id = mapperTestScheme.Id, SchemeName = mapperTestScheme.Name, ApprovalName = mapperTestAatf.ApprovalNumber },
                new SchemeData() { Id = mapperTestScheme2.Id, SchemeName = mapperTestScheme2.Name, ApprovalName = mapperTestAatf.ApprovalNumber }
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
            result.SchemeList.Count.Should().Be(2);
            result.SchemeList.ElementAt(0).SchemeId.Should().Be(mapperTestScheme.Id);
            result.SchemeList.ElementAt(0).SchemeName.Should().Be(mapperTestScheme.Name);
            result.SchemeList.ElementAt(0).Tonnages.B2B.Should().Be("2.468");
            result.SchemeList.ElementAt(0).Tonnages.B2C.Should().Be("2.468");
            result.SchemeList.ElementAt(1).SchemeId.Should().Be(mapperTestScheme2.Id);
            result.SchemeList.ElementAt(1).SchemeName.Should().Be(mapperTestScheme2.Name);
            result.SchemeList.ElementAt(1).Tonnages.B2B.Should().Be("0.468");
            result.SchemeList.ElementAt(1).Tonnages.B2C.Should().Be("4.468");
        }
    }
}
