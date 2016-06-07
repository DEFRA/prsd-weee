namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.Admin; 
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetProducerEeeDataHistoryCsvHandlerTests
    {
        [Fact]
        public async void CreateResults_EeeHistoryCsvResult_PopulatedWithCorrectValues()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            const int complianceYear = 2995;
            const string prn = "WEE/AW0101AW";

            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = complianceYear;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, prn);

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 1, (decimal)1.01);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 2, (decimal)1.02);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 3, (decimal)1.03);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 4, (decimal)1.04);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 5, (decimal)1.05);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 6, (decimal)1.06);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 7, (decimal)1.07);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 8, (decimal)1.08);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 9, (decimal)1.09);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 10, (decimal)1.10);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 11, (decimal)1.11);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 12, (decimal)1.12);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 13, (decimal)1.13);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 14, (decimal)1.14);

                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 1, (decimal)2.01);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 2, (decimal)2.02);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 3, (decimal)2.03);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 4, (decimal)2.04);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 5, (decimal)2.05);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 6, (decimal)2.06);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 7, (decimal)2.07);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 8, (decimal)2.08);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 9, (decimal)2.09);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 10, (decimal)2.10);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 11, (decimal)2.11);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 12, (decimal)2.12);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 13, (decimal)2.13);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 14, (decimal)2.14);

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

                    Assert.Equal(result.PRN, prn);
                    Assert.Equal(result.Quarter, 1);
                    Assert.Equal(result.ComplianceYear, complianceYear);

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
                }
            }
        }
    }
}
