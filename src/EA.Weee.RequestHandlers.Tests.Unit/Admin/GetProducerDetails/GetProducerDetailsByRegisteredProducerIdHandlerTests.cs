namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Domain;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
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
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                Domain.ObligationType.Both,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                new List<Domain.Producer.BrandName>(),
                new List<Domain.Producer.SICCode>(),
                A.Dummy<ChargeBandAmount>(),
                0);

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
            Assert.Equal(result.Prn, registeredProducer.ProducerRegistrationNumber);
        }
    }
}
