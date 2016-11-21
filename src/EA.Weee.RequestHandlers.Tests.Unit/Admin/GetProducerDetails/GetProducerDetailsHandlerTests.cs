namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Scheme;
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.Admin.GetProducerDetails;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Weee.Security;
    using Xunit;
    using GetProducerDetails = Requests.Admin.GetProducerDetails;

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
            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

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
        public async Task HandleAsync_WithNoRecordForRegistrationNumberAndComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA", 2016))
                .Returns(new List<ProducerSubmission>());

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA",
                ComplianceYear = 2016
            };

            // Act
            Func<Task<ProducerDetails>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA", 2017)).Returns(new List<Domain.Producer.ProducerSubmission>()
            {
                producer1,
                producer2
            });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA",
                ComplianceYear = 2017
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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.B2B,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.B2C,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch("WEE/AA1111AA", 2017)).Returns(new List<Domain.Producer.ProducerSubmission>()
            {
                producer1,
                producer2
            });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            Requests.Admin.GetProducerDetails request = new Requests.Admin.GetProducerDetails()
            {
                RegistrationNumber = "WEE/AA1111AA",
                ComplianceYear = 2017
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

        [Fact]
        public async Task HandleAync_ReturnsTrueForCanRemoveProducer_WhenCurrentUserIsInternalAdmin()
        {
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            registeredProducer.SetCurrentSubmission(producer);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(true);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            Assert.True(result.CanRemoveProducer);
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .MustHaveHappened();
        }

        [Fact]
        public async Task HandleAync_ReturnsFalseForCanRemoveProducer_WhenCurrentUserIsNotInternalAdmin()
        {
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            Assert.False(result.CanRemoveProducer);
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .MustHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_ForCompanyProducer_ReturnsCompanyContactDetails_AndIsCompanyValueTrue()
        {
            // Arrange
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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

            var producerAddress = new ProducerAddress("PrimaryName", "SecondaryName", "Street", "Town", "Locality", "AdministrativeArea", 
                new Country(A.Dummy<Guid>(), "TestCountry"), "PostCode");

            var producerContact = new ProducerContact(
                "Title", "Forename", "Surname", "123456", "1235467", "12345678", "a@b.c", producerAddress);

            var producerBusiness = new ProducerBusiness(
                companyDetails: new Company("TestCompany", "123", producerContact));

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness,
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schemes);

            ProducerContactDetails businessContact = result.Schemes[0].ProducerBusinessContact;
            Assert.Equal("PrimaryName, SecondaryName, Street, Town, Locality, AdministrativeArea, PostCode, TestCountry", businessContact.Address);
            Assert.Equal("Title Forename Surname", businessContact.ContactName);
            Assert.Equal("123456", businessContact.Telephone);
            Assert.Equal("1235467", businessContact.Mobile);
            Assert.Equal("a@b.c", businessContact.Email);
            Assert.True(result.Schemes[0].IsCompany);
        }

        [Fact]
        public async Task HandleAsync_ForPartnershipProducer_ReturnsPartnershipContactDetails_AndIsCompanyValueFalse()
        {
            // Arrange
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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

            var producerAddress = new ProducerAddress("PrimaryName", "SecondaryName", "Street", "Town", "Locality", "AdministrativeArea",
                new Country(A.Dummy<Guid>(), "TestCountry"), "PostCode");

            var producerContact = new ProducerContact(
                "Title", "Forename", "Surname", "123456", "1235467", "12345678", "a@b.c", producerAddress);

            var producerBusiness = new ProducerBusiness(
                partnership: new Partnership("TestPartnership", producerContact, new List<Partner> { }));

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness,
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schemes);

            ProducerContactDetails businessContact = result.Schemes[0].ProducerBusinessContact;
            Assert.Equal("PrimaryName, SecondaryName, Street, Town, Locality, AdministrativeArea, PostCode, TestCountry", businessContact.Address);
            Assert.Equal("Title Forename Surname", businessContact.ContactName);
            Assert.Equal("123456", businessContact.Telephone);
            Assert.Equal("1235467", businessContact.Mobile);
            Assert.Equal("a@b.c", businessContact.Email);
            Assert.False(result.Schemes[0].IsCompany);
        }

        [Fact]
        public async Task HandleAsync_ForUnknownBusinessType_ReturnsNullContactDetails_AndIsCompanyValueFalse()
        {
            // Arrange
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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

            var producerBusiness = new ProducerBusiness();

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness,
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schemes);

            Assert.Null(result.Schemes[0].ProducerBusinessContact);
            Assert.False(result.Schemes[0].IsCompany);
        }
        
        [Fact]
        public async Task Handle_Async_ReturnsNullCorrespondentForNotices_WhenCorrespondentForNoticesUnavailable()
        {  
            // Arrange
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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

            var producerBusiness = new ProducerBusiness();

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness,
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schemes);

            Assert.Null(result.Schemes[0].CorrespondentForNotices);
        }

        [Fact]
        public async Task Handle_Async_ReturnsCorrespondentForNoticesDetails()
        {
            // Arrange
            Scheme scheme = new Scheme(A.Dummy<Guid>());

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

            var correspondentForNoticesAddress = new ProducerAddress("PrimaryName", "SecondaryName", "Street", "Town", "Locality", "AdministrativeArea",
                new Country(A.Dummy<Guid>(), "TestCountry"), "PostCode");

            var correspondentForNotices = new ProducerContact(
                "Title", "Forename", "Surname", "123456", "98765", "123456", "a@b.c", correspondentForNoticesAddress);

            var producerBusiness = new ProducerBusiness(correspondentForNoticesContact: correspondentForNotices);

            var producer = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness,
                null,
                new DateTime(2015, 1, 1),
                0,
                false,
                null,
                "Trading Name 1",
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.Both,
                Domain.Obligation.ObligationType.Both,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<BrandName>(),
                new List<SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

            IGetProducerDetailsDataAccess dataAccess = A.Fake<IGetProducerDetailsDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<string>._, A<int>._))
                .Returns(new List<ProducerSubmission> { producer });

            IWeeeAuthorization authorization = A.Fake<IWeeeAuthorization>();
            A.CallTo(() => authorization.CheckUserInRole(Roles.InternalAdmin))
                .Returns(false);

            IMapper mapper = A.Fake<IMapper>();

            GetProducerDetailsHandler handler = new GetProducerDetailsHandler(dataAccess, authorization, mapper);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetails>());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Schemes);

            var details = result.Schemes[0];

            Assert.NotNull(details.CorrespondentForNotices);

            Assert.Equal("Title Forename Surname", details.CorrespondentForNotices.ContactName);
            Assert.Equal("a@b.c", details.CorrespondentForNotices.Email);
            Assert.Equal("123456", details.CorrespondentForNotices.Telephone);
            Assert.Equal("98765", details.CorrespondentForNotices.Mobile);
            Assert.Equal("PrimaryName, SecondaryName, Street, Town, Locality, AdministrativeArea, PostCode, TestCountry", details.CorrespondentForNotices.Address);
        }
    }
}
