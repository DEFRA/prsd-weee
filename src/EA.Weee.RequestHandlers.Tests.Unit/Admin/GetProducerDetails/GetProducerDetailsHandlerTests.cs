namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.Admin.GetProducerDetails;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;

    public class GetProducerDetailsHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External)]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            IGetProducerDetailsDataAccess dataAccess = A.Dummy<IGetProducerDetailsDataAccess>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            Func<Task<ProducerDetails>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownRegistrationNumber_ThrowsException()
        {
            // Arrange
            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA"))
                .Returns(new List<Domain.Producer.ProducerSubmission>());

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            Func<Task<ProducerDetails>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task HandleAsync_ProducerRegisteredIn2017_ReturnsProducerDetailsFor2017()
        {
            // Arrange
            Scheme scheme = new Scheme(
                A.Dummy<Guid>());

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                2017,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>());

            RegisteredProducer registeredProducer = new RegisteredProducer(
                "WEE/AA1111AA",
                2017,
                scheme);

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            registeredProducer.SetCurrentSubmission(producer);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA"))
                .Returns(new List<Domain.Producer.ProducerSubmission>() { producer });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ProducerDetails result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WEE/AA1111AA", result.RegistrationNumber);
            Assert.Equal(2017, result.ComplianceYear);
            Assert.Equal(1, result.Schemes.Count);
            Assert.Equal(scheme.SchemeName, result.Schemes[0].SchemeName);
        }

        [Fact]
        public async Task HandleAsync_ProducerRegisteredIn2016And2017_ReturnsProducerDetailsFor2017()
        {
            // Arrange
            var scheme = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.SchemeName).Returns("Scheme Name");

            var memberUpload1 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme);

            var producer1 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2016, scheme),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme);

            var producer2 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2017, scheme),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.ProducerSubmission>() 
            {
                producer1,
                producer2
            });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ProducerDetails result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WEE/AA1111AA", result.RegistrationNumber);
            Assert.Equal(2017, result.ComplianceYear);
            Assert.Equal(1, result.Schemes.Count);
            Assert.Equal("Scheme Name", result.Schemes[0].SchemeName);
        }

        [Fact]
        public async Task HandleAsync_ProducerRegisteredTwiceIn2017ForSameScheme_ReturnsLatestProducerDetailsWithFirstRegistrationDate()
        {
            // Arrange
            var scheme = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.SchemeName).Returns("Scheme Name");

            var memberUpload1 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme);

            var producer1 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2017, scheme),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme);

            var producer2 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2017, scheme),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 2),
                0,
                false,
                null,
                "Trading Name 2",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.ProducerSubmission>() 
            {
                producer1,
                producer2
            });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ProducerDetails result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Trading Name 2", result.Schemes[0].TradingName);
            Assert.Equal(new DateTime(2015, 1, 1), result.Schemes[0].RegistrationDate);
        }

        [Fact]
        public async Task HandleAsync_ProducerRegisteredTwiceIn2017ForDifferentSchemes_ReturnsProducerDetailsForBothSchemesOrderedBySchemeName()
        {
            // Arrange
            var scheme1 = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme1.SchemeName).Returns("Scheme Name 1");

            var memberUpload1 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme1);

            var producer1 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2017, scheme1),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.B2B,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            var scheme2 = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme2.SchemeName).Returns("Scheme Name 2");

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme2);

            var producer2 = new EA.Weee.Domain.Producer.ProducerSubmission(
                new Domain.Producer.RegisteredProducer("WEE/AA1111AA", 2017, scheme2),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 2",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.B2C,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.ProducerSubmission>() 
            {
                producer1,
                producer2
            });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA"
            };

            // Act
            ProducerDetails result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Schemes.Count);
            Assert.Collection(result.Schemes,
                r1 => Assert.Equal("Scheme Name 1", r1.SchemeName),
                r2 => Assert.Equal("Scheme Name 2", r2.SchemeName));
        }
    }
}
