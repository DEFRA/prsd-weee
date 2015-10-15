﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.Admin.GetProducerDetails;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
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
                .Returns(new List<Domain.Producer.Producer>());

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
            var scheme = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.SchemeName).Returns("Scheme Name");

           var memberUpload = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload.ComplianceYear).Returns(2017); 
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);

            var producer = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);
            
            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA"))
                .Returns(new List<Domain.Producer.Producer>() { producer });

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
        public async Task HandleAsync_ProducerRegisteredIn2016And2017_ReturnsProducerDetailsFor2017()
        {
            // Arrange
            var scheme = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.SchemeName).Returns("Scheme Name");

            var memberUpload1 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme);

            var producer1 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme);

            var producer2 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.Producer>() 
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

            var producer1 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme);

            var producer2 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 2),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 2",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.Producer>() 
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

            var producer1 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload1,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 1",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.B2B,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            var scheme2 = A.Fake<EA.Weee.Domain.Scheme.Scheme>();
            A.CallTo(() => scheme2.SchemeName).Returns("Scheme Name 2");

            var memberUpload2 = A.Fake<EA.Weee.Domain.Scheme.MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme2);

            var producer2 = new EA.Weee.Domain.Producer.Producer(
                new Guid(),
                memberUpload2,
                new EA.Weee.Domain.Producer.ProducerBusiness(),
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                "WEE/AA1111AA",
                null,
                "Trading Name 2",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.B2C,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                false,
                Domain.ChargeBandType.A,
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA")).Returns(new List<Domain.Producer.Producer>() 
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
