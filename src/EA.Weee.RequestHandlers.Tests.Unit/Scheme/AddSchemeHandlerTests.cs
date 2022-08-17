namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class AddSchemeHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDetailsDataAccess organisationDataAccess;
        private readonly IUpdateSchemeInformationDataAccess dataAccess;

        private readonly AddSchemeHandler handler;

        public AddSchemeHandlerTests()
        {
            authorization = AuthorizationBuilder.CreateUserWithAllRights();
            dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            organisationDataAccess = A.Fake<IOrganisationDetailsDataAccess>();

            handler = new AddSchemeHandler(authorization, dataAccess, organisationDataAccess);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            AddSchemeHandler handler = new AddSchemeHandler(authorization, dataAccess, organisationDataAccess);

            // Act
            Func<Task<CreateOrUpdateSchemeInformationResult>> testCode = async () => await handler.HandleAsync(A.Dummy<CreateScheme>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_EverythingCorrect_CreateSchemeCallsSaveAndReturnsSuccess()
        {
            // Act
            CreateScheme request = new CreateScheme(
                A.Dummy<Guid>(),
                "New scheme name",
                "WEE/AB8888CD/SCH",
                "WEE7453956",
                ObligationType.B2B,
                new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                SchemeStatus.Approved); 

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();

            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_EverythingCorrectButWithNewOrganisation_CreateSchemeCallsSaveAndReturnsSuccess_CompleteOrgCalled()
        {
            Organisation organisation = A.Fake<Organisation>();
            organisation.OrganisationStatus = Domain.Organisation.OrganisationStatus.Incomplete;

            // Act
            CreateScheme request = new CreateScheme(
                organisation.Id,
                "New scheme name",
                "WEE/AB8888CD/SCH",
                "WEE7453956",
                ObligationType.B2B,
                new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                SchemeStatus.Approved);

            A.CallTo(() => organisationDataAccess.FetchOrganisationAsync(organisation.Id)).Returns(organisation);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();
            A.CallTo(() => organisationDataAccess.SaveAsync()).MustHaveHappened();

            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_EverythingCorrectButWithExistingOrganisation_CreateSchemeCallsSaveAndReturnsSuccess_CompleteOrgNotCalled()
        {
            Organisation organisation = A.Fake<Organisation>();
            organisation.OrganisationStatus = Domain.Organisation.OrganisationStatus.Complete;

            // Act
            CreateScheme request = new CreateScheme(
                organisation.Id,
                "New scheme name",
                "WEE/AB8888CD/SCH",
                "WEE7453956",
                ObligationType.B2B,
                new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                SchemeStatus.Approved);

            A.CallTo(() => organisationDataAccess.FetchOrganisationAsync(organisation.Id)).Returns(organisation);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();
            A.CallTo(() => organisationDataAccess.SaveAsync()).MustNotHaveHappened();

            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_WhereSchemeAlreadyExistsForOrganisation_ReturnsFailureResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));
            Organisation organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Schemes).Returns(new List<Scheme>() { A.Fake<Scheme>() });

            A.CallTo(() => organisationDataAccess.FetchOrganisationAsync(A<Guid>._)).Returns(organisation);

            // Act
            CreateScheme request = new CreateScheme(
                A.Dummy<Guid>(),
                "New scheme name",
                "WEE/ZZ9999ZZ/SCH",
                "WEE7453956",
                ObligationType.B2B,
                new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.SchemeAlreadyExists, result.Result);
        }

        [Fact]
        public async Task HandleAsync_WhereApprovalNumberIsAValueThatAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            A.CallTo(() => dataAccess.CheckSchemeApprovalNumberInUseAsync("WEE/ZZ9999ZZ/SCH")).Returns(true);

            // Act
            CreateScheme request = new CreateScheme(
                 A.Dummy<Guid>(),
                 "New scheme name",
                 "WEE/ZZ9999ZZ/SCH",
                 "WEE7453956",
                 ObligationType.B2B,
                 new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                 SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingEASchemeIbisCustomerReferenceNumberToAValueThatAlreadyExistsForAnotherEAScheme_ReturnsFailureResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE7453846",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            Scheme otherScheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(otherScheme, new Guid("C78D98A9-E33A-4E20-88D3-F1C99D5165B1"));
            otherScheme.UpdateScheme(
                "Scheme 2",
                "WEE/BB1111BB/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            otherScheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme, otherScheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure, result.Result);

            Assert.NotNull(result.IbisCustomerReferenceUniquenessFailure);
            Assert.Equal("WEE8643759", result.IbisCustomerReferenceUniquenessFailure.IbisCustomerReference);
            Assert.Equal("WEE/BB1111BB/SCH", result.IbisCustomerReferenceUniquenessFailure.OtherSchemeApprovalNumber);
            Assert.Equal("Scheme 2", result.IbisCustomerReferenceUniquenessFailure.OtherSchemeName);
        }

        [Fact]
        public async Task HandleAsync_SettingEASchemeIbisCustomerReferenceNumberToAValueThatAlreadyExistsForAnotherNonEAScheme_ReturnsSuccessResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            UKCompetentAuthority devlovedAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(devlovedAgency, new Guid("76BE711C-191D-4648-AFE7-4404D287535C"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE7453846",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            Scheme otherScheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(otherScheme, new Guid("C78D98A9-E33A-4E20-88D3-F1C99D5165B1"));
            otherScheme.UpdateScheme(
                "Scheme 2",
                "WEE/BB1111BB/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                devlovedAgency);
            otherScheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingNonEASchemeIbisCustomerReferenceNumberToAValueThatAlreadyExistsForAnotherScheme_ReturnsSuccessResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            UKCompetentAuthority devlovedAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(devlovedAgency, new Guid("76BE711C-191D-4648-AFE7-4404D287535C"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE7453846",
                Domain.Obligation.ObligationType.B2C,
                devlovedAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            Scheme otherScheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(otherScheme, new Guid("C78D98A9-E33A-4E20-88D3-F1C99D5165B1"));
            otherScheme.UpdateScheme(
                "Scheme 2",
                "WEE/BB1111BB/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                devlovedAgency);
            otherScheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>());

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("76BE711C-191D-4648-AFE7-4404D287535C"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingEASchemeIbisCustomerReferenceNumberToNull_ReturnsFailureResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                null,
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingEASchemeIbisCustomerReferenceNumberToEmptyString_ReturnsFailureResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                string.Empty,
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingNonEASchemeIbisCustomerReferenceNumberToNull_ReturnsSuccessResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                null,
                ObligationType.B2C,
                new Guid("61D93F16-A478-4F45-AE6B-2A581F0C0648"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_SettingNonEASchemeIbisCustomerReferenceNumberToEmptyString_ReturnsSuccessResult()
        {
            // Arrange
            UKCompetentAuthority environmentAgency = A.Dummy<UKCompetentAuthority>();
            typeof(UKCompetentAuthority).GetProperty("Id").SetValue(environmentAgency, new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"));

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(scheme, new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"));
            scheme.UpdateScheme(
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            scheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            // Act
            CreateScheme request = new CreateScheme(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                string.Empty,
                ObligationType.B2C,
                new Guid("61D93F16-A478-4F45-AE6B-2A581F0C0648"),
                SchemeStatus.Approved);

            CreateOrUpdateSchemeInformationResult result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CreateOrUpdateSchemeInformationResult.ResultType.Success, result.Result);
        }
    }
}
