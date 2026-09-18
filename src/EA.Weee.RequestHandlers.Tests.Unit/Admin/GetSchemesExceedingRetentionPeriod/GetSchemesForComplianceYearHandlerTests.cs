namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSchemesExceedingRetentionPeriod
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Security;

    public class GetSchemesForComplianceYearHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesForComplianceYearDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;

        public GetSchemesForComplianceYearHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = CreateFakeDataAccess();
            this.schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            this.context = A.Fake<WeeeContext>();
        }

        private IGetSchemesForComplianceYearDataAccess CreateFakeDataAccess()
        {
            IGetSchemesForComplianceYearDataAccess dataAccess = A.Fake<IGetSchemesForComplianceYearDataAccess>();

            var schemeName1 = "AAA";
            var schemeName2 = "CCC";
            var schemeName3 = "EEE";

            var results = new List<string>()
            {
                schemeName1,
                schemeName2,
                schemeName3
            };

            A.CallTo(() => dataAccess.GetItemsAsync(2018)).Returns(results);
            return dataAccess;
        }
    }
}
