namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using Core.Shared;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.DataReturns;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetProducerEeeDataHistoryCsvHandlerTests
    {
        [Fact]
        public async Task CreateResults_EeeHistoryCsvResult_PopulatedWithCorrectValues()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            int complianceYear = DateTime.Now.Year;
            const string prn = "WEE/AW0101AW";

            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = complianceYear;

                var categories = EnumHelper.GetValues(typeof(WeeeCategory));
                var maxCategoryId = categories.Max(x => x.Key);
                decimal b2cTonnage = 1;
                decimal b2bTonnage = 2;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, prn);

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear, 1);

                for (int i = 1; i <= maxCategoryId; i++)
                {
                    b2cTonnage = b2cTonnage + (decimal)0.01;
                    b2bTonnage = b2bTonnage + (decimal)0.01;

                    helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", i, b2cTonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", i, b2bTonnage);
                }

                database.Model.SaveChanges();

                // Act
                var dataAccess = new GetProducerEeeDataHistoryCsvHandler(authorization, database.WeeeContext, csvWriterFactory);
                var items = await database.WeeeContext.StoredProcedures.SpgProducerEeeHistoryCsvData(prn);
                IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> results = dataAccess.CreateResults(items);

                // Assert
                Assert.Single(results);
                using (IEnumerator<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> iterator = results.GetEnumerator())
                {
                    iterator.MoveNext();
                    GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult result = iterator.Current;

                    Assert.Equal(prn, result.PRN);
                    Assert.Equal(1, result.Quarter);
                    Assert.Equal(complianceYear, result.ComplianceYear);

                    Assert.Equal(result.Cat1B2C, (decimal)1.01);
                    Assert.Equal(result.Cat2B2C, (decimal)1.02);
                    Assert.Equal(result.Cat3B2C, (decimal)1.03);
                    Assert.Equal(result.Cat4B2C, (decimal)1.04);
                    Assert.Equal(result.Cat5B2C, (decimal)1.05);
                    Assert.Equal(result.Cat6B2C, (decimal)1.06);
                    Assert.Equal(result.Cat7B2C, (decimal)1.07);
                    Assert.Equal(result.Cat8B2C, (decimal)1.08);
                    Assert.Equal(result.Cat9B2C, (decimal)1.09);
                    Assert.Equal(result.Cat10B2C, (decimal)1.10);
                    Assert.Equal(result.Cat11B2C, (decimal)1.11);
                    Assert.Equal(result.Cat12B2C, (decimal)1.12);
                    Assert.Equal(result.Cat13B2C, (decimal)1.13);
                    Assert.Equal(result.Cat14B2C, (decimal)1.14);
                    Assert.Equal(result.Cat15B2C, (decimal)1.15);

                    Assert.Equal(result.Cat1B2B, (decimal)2.01);
                    Assert.Equal(result.Cat2B2B, (decimal)2.02);
                    Assert.Equal(result.Cat3B2B, (decimal)2.03);
                    Assert.Equal(result.Cat4B2B, (decimal)2.04);
                    Assert.Equal(result.Cat5B2B, (decimal)2.05);
                    Assert.Equal(result.Cat6B2B, (decimal)2.06);
                    Assert.Equal(result.Cat7B2B, (decimal)2.07);
                    Assert.Equal(result.Cat8B2B, (decimal)2.08);
                    Assert.Equal(result.Cat9B2B, (decimal)2.09);
                    Assert.Equal(result.Cat10B2B, (decimal)2.10);
                    Assert.Equal(result.Cat11B2B, (decimal)2.11);
                    Assert.Equal(result.Cat12B2B, (decimal)2.12);
                    Assert.Equal(result.Cat13B2B, (decimal)2.13);
                    Assert.Equal(result.Cat14B2B, (decimal)2.14);
                    Assert.Equal(result.Cat15B2B, (decimal)2.15);
                }
            }
        }
    }
}
