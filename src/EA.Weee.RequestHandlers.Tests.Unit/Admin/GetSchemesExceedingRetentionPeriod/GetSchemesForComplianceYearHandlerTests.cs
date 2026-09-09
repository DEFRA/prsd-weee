namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod;
    using EA.Weee.Requests;
    using FakeItEasy;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Admin.GetSchemes;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Weee.Domain.Organisation;
    using Weee.Tests.Core;
    using Xunit;
    using static EA.Weee.Requests.Admin.GetSchemes;
    using ObligationType = Domain.Obligation.ObligationType;
    using SchemeStatus = Domain.Scheme.SchemeStatus;

    public class GetSchemesForComplianceYearHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesForComplianceYearDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly WeeeContext context;
        private readonly IGetSchemeData getSchemeDataStub;

        public GetSchemesForComplianceYearHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = CreateFakeDataAccess();
            this.schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            this.context = A.Fake<WeeeContext>();
            this.getSchemeDataStub = CreateFakeSchemeDataStub();
        }

        /// <summary>
        /// </summary>
        [Fact]
        public async Task HandleAsync_WhenSchemesExistForComplianceYear_ReturnsSchemes()
        {
            // Arrange
            GetSchemesForComplianceYear request = new GetSchemesForComplianceYear(2018);
            GetSchemesForComplianceYearHandler handler = new GetSchemesForComplianceYearHandler(authorization, schemeMap, context, dataAccess, getSchemeDataStub);

            // Act
            List<SchemeData> results = await handler.HandleAsync(request);

            // Assert
            Assert.Collection(
                results,
                (element1) => Assert.Equal(schemeData1, element1),
                (element2) => Assert.Equal(schemeData3, element2),
                (element3) => Assert.Equal(schemeData5, element3));
        }

        private SchemeData schemeData1;
        private SchemeData schemeData2;
        private SchemeData schemeData3;
        private SchemeData schemeData4;
        private SchemeData schemeData5;

        private IGetSchemeData CreateFakeSchemeDataStub()
        {
            //IGetSchemeData getSchemeData = A.Fake<IGetSchemeData>();

            schemeData1 = A.Fake<SchemeData>();
            schemeData1.Name = "MCH";

            schemeData2 = A.Fake<SchemeData>();
            schemeData2.Name = "ARB";

            schemeData3 = A.Fake<SchemeData>();
            schemeData3.Name = "ZRS";

            schemeData4 = A.Fake<SchemeData>();
            schemeData4.Name = "BBD";

            schemeData5 = A.Fake<SchemeData>();
            schemeData5.Name = "HRT";

            var listSchemeData = new List<SchemeData>();
            listSchemeData.Add(schemeData1);
            listSchemeData.Add(schemeData2);
            listSchemeData.Add(schemeData3);
            listSchemeData.Add(schemeData4);
            listSchemeData.Add(schemeData5);

            //A.CallTo(() => getSchemeData.GetSchemeData()).Returns(listSchemeData);

            IGetSchemeData getSchemeData = new GetSchemeDataStub(listSchemeData);
                
            return getSchemeData;
        }

        private IGetSchemesForComplianceYearDataAccess CreateFakeDataAccess()
        {
            IGetSchemesForComplianceYearDataAccess dataAccess = A.Fake<IGetSchemesForComplianceYearDataAccess>();

            var schemeName1 = "HRT";
            var schemeName2 = "ZRS";
            var schemeName3 = "MCH";

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
