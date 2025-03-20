namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchIssuedChargesCsv
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Domain.Producer.Classification;
    using EA.Weee.Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Charges.FetchIssuedChargesCsv;
    using RequestHandlers.Security;
    using Requests.Charges;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchIssuedChargesCsvHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var dataAccess = A.Dummy<IFetchIssuedChargesCsvDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchIssuedChargesCsvHandler(authorization, dataAccess, csvWriterFactory);

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<FetchIssuedChargesCsv>()));
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Dummy<IFetchIssuedChargesCsvDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchIssuedChargesCsvHandler(authorization, dataAccess, csvWriterFactory);

            var data = await handler.HandleAsync(A.Dummy<FetchIssuedChargesCsv>());

            Assert.NotEmpty(data.Data);

            var result = Encoding.UTF8.GetString(data.Data);

            var lines = result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var expectedHeaders = string.Join(",", new[]
            {
            "Scheme name",
            "Compliance year",
            "Submission date and time (GMT)",
            "Producer name",
            "PRN",
            "Charge value (GBP)",
            "Charge band",
            "Selling technique",
            "Online market places charge value",
            "Issued date",
            "Reg. Off. or PPoB country",
            "Includes annual charge"
            });

            Assert.Equal(expectedHeaders, lines[0]);
        }

        [Fact]
        public async Task HandleAsync_PutsChargeValueInCorrectColumn()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Fake<IFetchIssuedChargesCsvDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var producerSubmissions = new[]
            {
                CreateDummyProducerSubmission(1m, SellingTechniqueType.DirectSellingtoEndUser),
                CreateDummyProducerSubmission(2m, SellingTechniqueType.IndirectSellingtoEndUser),
                CreateDummyProducerSubmission(3m, SellingTechniqueType.Both),
                CreateDummyProducerSubmission(4m, SellingTechniqueType.OnlineMarketplacesAndFulfilmentHouses),
            };

            A.CallTo(() => dataAccess.FetchInvoicedProducerSubmissionsAsync(A<UKCompetentAuthority>.Ignored, A<int>.Ignored, A<Guid>.Ignored)).Returns(Task.FromResult<IEnumerable<ProducerSubmission>>(producerSubmissions));

            var handler = new FetchIssuedChargesCsvHandler(authorization, dataAccess, csvWriterFactory);

            var data = await handler.HandleAsync(A.Dummy<FetchIssuedChargesCsv>());

            Assert.NotEmpty(data.Data);

            var result = Encoding.UTF8.GetString(data.Data);

            var lines = result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var expectedHeaders = string.Join(",", new[]
            {
            "Scheme name",
            "Compliance year",
            "Submission date and time (GMT)",
            "Producer name",
            "PRN",
            "Charge value (GBP)",
            "Charge band",
            "Selling technique",
            "Online market places charge value",
            "Issued date",
            "Reg. Off. or PPoB country",
            "Includes annual charge"
            });

            Assert.Equal(expectedHeaders, lines[0]);

            var row1Data = lines[1].Split(',');
            var row2Data = lines[2].Split(',');
            var row3Data = lines[3].Split(',');
            var row4Data = lines[4].Split(',');

            // for direct, indirect and both (all non-OMP ones), the old Charge Value column should be used
            // for OMP, the new OMP charge value column should be used
            // in either case, the other should be blank

            Assert.Equal(decimal.Parse(row1Data[5]), producerSubmissions[0].ChargeThisUpdate);
            Assert.Equal(row1Data[7], EnumHelper.GetDisplayName(Enumeration.FromValue<SellingTechniqueType>(producerSubmissions[0].SellingTechniqueType)));
            Assert.Equal(row1Data[8], string.Empty);
            Assert.Equal(decimal.Parse(row2Data[5]), producerSubmissions[1].ChargeThisUpdate);
            Assert.Equal(row2Data[7], EnumHelper.GetDisplayName(Enumeration.FromValue<SellingTechniqueType>(producerSubmissions[1].SellingTechniqueType)));
            Assert.Equal(row2Data[8], string.Empty);
            Assert.Equal(decimal.Parse(row3Data[5]), producerSubmissions[2].ChargeThisUpdate);
            Assert.Equal(row3Data[7], EnumHelper.GetDisplayName(Enumeration.FromValue<SellingTechniqueType>(producerSubmissions[2].SellingTechniqueType)));
            Assert.Equal(row3Data[8], string.Empty);
            Assert.Equal(row4Data[5], string.Empty);
            Assert.Equal(row4Data[7], EnumHelper.GetDisplayName(Enumeration.FromValue<SellingTechniqueType>(producerSubmissions[3].SellingTechniqueType)));
            Assert.Equal(decimal.Parse(row4Data[8]), producerSubmissions[3].ChargeThisUpdate);
        }

        private static ProducerSubmission CreateDummyProducerSubmission(decimal value, SellingTechniqueType sellingTechnique)
        {
            return new ProducerSubmission(A.Dummy<RegisteredProducer>(),
                            A.Dummy<MemberUpload>(),
                            A.Dummy<ProducerBusiness>(),
                            A.Dummy<AuthorisedRepresentative>(),
                            A.Dummy<DateTime>(),
                            A.Dummy<decimal?>(),
                            A.Dummy<bool>(),
                            A.Dummy<DateTime?>(),
                            A.Dummy<string>(),
                            A.Dummy<EEEPlacedOnMarketBandType>(),
                            sellingTechnique,
                            A.Dummy<Domain.Obligation.ObligationType>(),
                            A.Dummy<AnnualTurnOverBandType>(),
                            A.Dummy<List<BrandName>>(),
                            A.Dummy<List<SICCode>>(),
                            A.Dummy<ChargeBandAmount>(),
                            value,
                            A.Dummy<StatusType>());
        }
    }
}
