﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using Core.Admin;
    using Domain;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetProducerDetailsByRegisteredProducerIdHandlerTests
    {
        [Theory]
        [InlineData(AuthorizationBuilder.UserType.External)]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            // Arrange
            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Dummy<IGetProducerDetailsByRegisteredProducerIdDataAccess>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateFromUserType(userType);

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetailsByRegisteredProducerId request =
                new Requests.Admin.GetProducerDetailsByRegisteredProducerId(Guid.NewGuid());

            // Act
            Func<Task<ProducerDetailsScheme>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownRegisteredProducerId_ThrowsException()
        {
            // Arrange
            var registeredProducerId = Guid.NewGuid();
            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();
            A.CallTo(() => dataAccess.Fetch(registeredProducerId))
                .Returns((Domain.Producer.RegisteredProducer)null);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetailsByRegisteredProducerId request =
                new Requests.Admin.GetProducerDetailsByRegisteredProducerId(registeredProducerId);

            // Act
            Func<Task<ProducerDetailsScheme>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task HandleAsync_HappyPath_ReturnsProducerDetailsSchemeObjectWithPerfectValue()
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
                new EA.Weee.Domain.Producer.ProducerBusiness(
                    new Company("CompanyName", "RegisteredNo",
                    new ProducerContact(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(),
                    new ProducerAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(),
                    new Country(Guid.NewGuid(), A.Dummy<string>()), A.Dummy<string>())))),
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
                0,
                A.Dummy<StatusType>());

            registeredProducer.SetCurrentSubmission(producer);

            registeredProducer.Scheme.UpdateScheme("SchemeName", "WEE/FA9999KE/SCH", "test", ObligationType.B2B, Guid.NewGuid());

            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();
            A.CallTo(() => dataAccess.Fetch(registeredProducer.Id))
                .Returns(registeredProducer);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            Requests.Admin.GetProducerDetailsByRegisteredProducerId request =
                new Requests.Admin.GetProducerDetailsByRegisteredProducerId(registeredProducer.Id);

            // Act
            ProducerDetailsScheme result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(result.ComplianceYear, registeredProducer.ComplianceYear);
            Assert.Equal(result.ProducerName, registeredProducer.CurrentSubmission.OrganisationName);
            Assert.Equal(result.SchemeName, registeredProducer.Scheme.SchemeName);
            Assert.Equal(result.RegistrationNumber, registeredProducer.ProducerRegistrationNumber);
        }

        /// <summary>
        /// This test ensures that the "HasSubmittedEEE" property of the ProducerDetailsScheme result
        /// is set to true when a submitted data return exists for the scheme and compliance year
        /// where an EEE output amount is included for the specified registered producer.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithDataReturnWithEee_SetsHasSubmittedEEEToTrue()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Organisation organisation = Organisation.CreateSoleTrader("Trading Name");

            Scheme scheme = new Scheme(organisation);

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/AA1111AA", 2016, scheme);

            Guid registeredProducerId = new Guid("75B6B4E7-BA92-477D-A6CA-C43C8C0E9823");
            typeof(Entity).GetProperty("Id").SetValue(registeredProducer, registeredProducerId);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                "data",
                new List<MemberUploadError>(),
                0,
                2016,
                scheme,
                "file.xml",
                "UserID");

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                A.Dummy<decimal>(),
                A.Dummy<StatusType>());

            registeredProducer.SetCurrentSubmission(producerSubmission);

            DataReturn dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q4));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            EeeOutputAmount eeeOutputAmount = new EeeOutputAmount(
                ObligationType.B2C,
                WeeeCategory.AutomaticDispensers,
                123.45m,
                registeredProducer);
            dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);

            dataReturnVersion.Submit("UserID");

            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();

            A.CallTo(() => dataAccess.Fetch(registeredProducerId)).Returns(registeredProducer);
            A.CallTo(() => dataAccess.FetchDataReturns(scheme, 2016)).Returns(new List<DataReturn>() { dataReturn });

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
            ProducerDetailsScheme result = await handler.HandleAsync(request);

            // Assert
            Assert.True(result.HasSubmittedEEE);
        }

        /// <summary>
        /// This test ensures that the "HasSubmittedEEE" property of the ProducerDetailsScheme result
        /// is set to false when a data return exists for the scheme and compliance year
        /// where an EEE output amount is included for the specified registered producer, but
        /// the data return version is not submitted.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithDataReturnWithEeeButNotSubmitted_SetsHasSubmittedEEEToFalse()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Organisation organisation = Organisation.CreateSoleTrader("Trading Name");

            Scheme scheme = new Scheme(organisation);

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/AA1111AA", 2016, scheme);

            Guid registeredProducerId = new Guid("75B6B4E7-BA92-477D-A6CA-C43C8C0E9823");
            typeof(Entity).GetProperty("Id").SetValue(registeredProducer, registeredProducerId);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                "data",
                new List<MemberUploadError>(),
                0,
                2016,
                scheme,
                "file.xml",
                "UserID");

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                A.Dummy<decimal>(),
                A.Dummy<StatusType>());

            registeredProducer.SetCurrentSubmission(producerSubmission);
            DataReturn dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q4));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            EeeOutputAmount eeeOutputAmount = new EeeOutputAmount(
                ObligationType.B2C,
                WeeeCategory.AutomaticDispensers,
                123.45m,
                registeredProducer);
            dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);

            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();

            A.CallTo(() => dataAccess.Fetch(registeredProducerId)).Returns(registeredProducer);
            A.CallTo(() => dataAccess.FetchDataReturns(scheme, 2016)).Returns(new List<DataReturn>() { dataReturn });

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
            ProducerDetailsScheme result = await handler.HandleAsync(request);

            // Assert
            Assert.False(result.HasSubmittedEEE);
        }

        /// <summary>
        /// This test ensures that the "HasSubmittedEEE" property of the ProducerDetailsScheme result
        /// is set to false when a submitted data return exists for the scheme and compliance year
        /// but no EEE output amounts are included for the specified registered producer.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithDataReturnWithEmptyEee_SetsHasSubmittedEEEToFalse()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Organisation organisation = Organisation.CreateSoleTrader("Trading Name");

            Scheme scheme = new Scheme(organisation);

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/AA1111AA", 2016, scheme);

            Guid registeredProducerId = new Guid("75B6B4E7-BA92-477D-A6CA-C43C8C0E9823");
            typeof(Entity).GetProperty("Id").SetValue(registeredProducer, registeredProducerId);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                "data",
                new List<MemberUploadError>(),
                0,
                2016,
                scheme,
                "file.xml",
                "UserID");

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                A.Dummy<decimal>(),
                A.Dummy<StatusType>());

            registeredProducer.SetCurrentSubmission(producerSubmission);
            DataReturn dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q4));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            dataReturnVersion.Submit("UserID");

            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();

            A.CallTo(() => dataAccess.Fetch(registeredProducerId)).Returns(registeredProducer);
            A.CallTo(() => dataAccess.FetchDataReturns(scheme, 2016)).Returns(new List<DataReturn>() { dataReturn });

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
            ProducerDetailsScheme result = await handler.HandleAsync(request);

            // Assert
            Assert.False(result.HasSubmittedEEE);
        }

        /// <summary>
        /// This test ensures that the "HasSubmittedEEE" property of the ProducerDetailsScheme result
        /// is set to false when a submitted data return exists for the scheme and compliance year
        /// but no EEE output amounts are included for the specified registered producer.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithDataReturnWithNullEee_SetsHasSubmittedEEEToFalse()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Organisation organisation = Organisation.CreateSoleTrader("Trading Name");

            Scheme scheme = new Scheme(organisation);

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/AA1111AA", 2016, scheme);

            Guid registeredProducerId = new Guid("75B6B4E7-BA92-477D-A6CA-C43C8C0E9823");
            typeof(Entity).GetProperty("Id").SetValue(registeredProducer, registeredProducerId);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                "data",
                new List<MemberUploadError>(),
                0,
                2016,
                scheme,
                "file.xml",
                "UserID");

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                A.Dummy<decimal>(),
                A.Dummy<StatusType>());

            registeredProducer.SetCurrentSubmission(producerSubmission);
            DataReturn dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q4));

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn,
                new WeeeCollectedReturnVersion(), new WeeeDeliveredReturnVersion(), null);

            dataReturnVersion.Submit("UserID");

            IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();

            A.CallTo(() => dataAccess.Fetch(registeredProducerId)).Returns(registeredProducer);
            A.CallTo(() => dataAccess.FetchDataReturns(scheme, 2016)).Returns(new List<DataReturn>() { dataReturn });

            GetProducerDetailsByRegisteredProducerIdHandler handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            GetProducerDetailsByRegisteredProducerId request = new GetProducerDetailsByRegisteredProducerId(registeredProducerId);
            ProducerDetailsScheme result = await handler.HandleAsync(request);

            // Assert
            Assert.False(result.HasSubmittedEEE);
        }

        [Fact]
        public async Task HandleAync_ReturnsTrueForCanRemoveProducer_WhenCurrentUserIsInternalAdmin()
        {
            // Arrange
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .AllowRole(Roles.InternalAdmin)
                .Build();

            var dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<Guid>._))
                .Returns(A.Dummy<RegisteredProducer>());

            var handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetailsByRegisteredProducerId>());

            // Assert
            Assert.True(result.CanRemoveProducer);
        }

        [Fact]
        public async Task HandleAync_ReturnsFalseForCanRemoveProducer_WhenCurrentUserIsNotInternalAdmin()
        {
            // Arrange
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var dataAccess = A.Fake<IGetProducerDetailsByRegisteredProducerIdDataAccess>();
            A.CallTo(() => dataAccess.Fetch(A<Guid>._))
                .Returns(A.Dummy<RegisteredProducer>());

            var handler = new GetProducerDetailsByRegisteredProducerIdHandler(dataAccess, authorization);

            // Act
            var result = await handler.HandleAsync(A.Dummy<GetProducerDetailsByRegisteredProducerId>());

            // Assert
            Assert.False(result.CanRemoveProducer);
        }
    }
}
