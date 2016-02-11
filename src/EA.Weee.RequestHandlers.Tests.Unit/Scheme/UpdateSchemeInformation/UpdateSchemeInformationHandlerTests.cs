namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.UpdateSchemeInformation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Scheme.UpdateSchemeInformation;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class UpdateSchemeInformationHandlerTests
    {
        /// <summary>
        /// This test ensures that a user without the right to access the internal area cannot use
        /// the UpdateSchemeInformationRequestHandler, and that any attempt to do so will result
        /// in a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenyInternalAreaAccess()
                .Build();

            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                A.Dummy<IUpdateSchemeInformationDataAccess>());

            // Act
            Func<Task<UpdateSchemeInformationResult>> testCode = async () => await handler.HandleAsync(A.Dummy<UpdateSchemeInformation>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures the happy path successfully updates the scheme domain entity
        /// with the specified information, that the scheme entity is persisted, and that
        /// a success message is returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_HappyPath_UpdatesSchemeCallsSaveAndReturnsSuccess()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();

            Scheme scheme = new Scheme(A.Dummy<Organisation>());
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._)).Returns(scheme);

            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                A.Dummy<Guid>(),
                "New scheme name",
                "WEE/AB8888CD/SCH",
                "WEE7453956",
                ObligationType.B2B,
                new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal("New scheme name", scheme.SchemeName);
            Assert.Equal("WEE/AB8888CD/SCH", scheme.ApprovalNumber);
            Assert.Equal("WEE7453956", scheme.IbisCustomerReference);
            Assert.Equal(Domain.Obligation.ObligationType.B2B, scheme.ObligationType);
            Assert.Equal(new Guid("559B69CE-865C-465F-89ED-D6A58AA8B0B9"), scheme.CompetentAuthorityId);
            Assert.Equal(Domain.Scheme.SchemeStatus.Approved, scheme.SchemeStatus);

            A.CallTo(() => dataAccess.SaveAsync()).MustHaveHappened();

            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        /// <summary>
        /// This test ensures that the approval number cannot be changed to a value that already exists.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WhereApprovalNumberIsChangingToAValueThatAlreadyExists_ReturnsFailureResult()
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.CheckSchemeApprovalNumberInUseAsync("WEE/ZZ9999ZZ/SCH")).Returns(true);

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/ZZ9999ZZ/SCH",
                "WEE7453846",
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure, result.Result);
        }

        /// <summary>
        /// This test ensures that the 1B1S customer billing reference must be unique accross all
        /// schemes within the Environment Agency.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme, otherScheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure, result.Result);

            Assert.NotNull(result.IbisCustomerReferenceUniquenessFailure);
            Assert.Equal("WEE8643759", result.IbisCustomerReferenceUniquenessFailure.IbisCustomerReference);
            Assert.Equal("WEE/BB1111BB/SCH", result.IbisCustomerReferenceUniquenessFailure.OtherSchemeApprovalNumber);
            Assert.Equal("Scheme 2", result.IbisCustomerReferenceUniquenessFailure.OtherSchemeName);
        }

        /// <summary>
        /// This test ensures that a scheme in the Environment Agency and a scheme under a devloved agency
        /// may have the same 1B1S customer billing reference.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        /// <summary>
        /// This test ensures that a scheme in a devloved agency and any other scheme
        /// may have the same 1B1S customer billing reference.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>());

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                "WEE8643759",
                ObligationType.B2C,
                new Guid("76BE711C-191D-4648-AFE7-4404D287535C"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        /// <summary>
        /// This test ensures that the 1B1S customer billing reference for an EA scheme cannot be set to null
        /// via this handler. Note: The value will be null when the scheme is first created.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                null,
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure, result.Result);
        }

        /// <summary>
        /// This test ensures that the 1B1S customer billing reference for an EA scheme cannot be set to a
        /// blank string via this handler. Note: The value will be null when the scheme is first created.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                string.Empty,
                ObligationType.B2C,
                new Guid("42D3130C-4CDB-4F74-866A-BFF839A347B5"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure, result.Result);
        }

        /// <summary>
        /// This test ensures that the restriction on schemes within the Environment Agency to have non-null
        /// 1B1S customer billing references does not affect schemes in devolved agencies.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                null,
                ObligationType.B2C,
                new Guid("61D93F16-A478-4F45-AE6B-2A581F0C0648"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        /// <summary>
        /// This test ensures that the restriction on schemes within the Environment Agency to have non-blank
        /// 1B1S customer billing references does not affect schemes in devolved agencies.
        /// </summary>
        /// <returns></returns>
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

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { scheme });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(
                authorization,
                dataAccess);

            // Act
            UpdateSchemeInformation request = new UpdateSchemeInformation(
                scheme.Id,
                "Scheme 1",
                "WEE/AA0000AA/SCH",
                string.Empty,
                ObligationType.B2C,
                new Guid("61D93F16-A478-4F45-AE6B-2A581F0C0648"),
                SchemeStatus.Approved);

            UpdateSchemeInformationResult result = await handler.HandleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }
    }
}