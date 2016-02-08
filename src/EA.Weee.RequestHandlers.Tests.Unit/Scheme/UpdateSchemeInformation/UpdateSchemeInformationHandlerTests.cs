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

        [Fact]
        public async Task HandleAsync_WhereIbisCustomerReferenceNumberIsChangingToAValueThatAlreadyExists_ReturnsFailureResult()
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
                .Returns(new List<Scheme>() { otherScheme });

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

        [Fact]
        public async Task HandleAsync_WhereIbisCustomerReferenceNumberIsNotChangingButEqualsAValueThatAlreadyExists_ReturnsSuccessResult()
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
                .Returns(new List<Scheme>() { otherScheme });

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

        [Fact]
        public async Task HandleAsync_WhereIbisCustomerReferenceNumberIsChangingToNullAndNullAlreadyExists_ReturnsSuccessResult()
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

            Scheme otherScheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(otherScheme, new Guid("C78D98A9-E33A-4E20-88D3-F1C99D5165B1"));
            otherScheme.UpdateScheme(
                "Scheme 2",
                "WEE/BB1111BB/SCH",
                null,
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            otherScheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { otherScheme });

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
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }

        [Fact]
        public async Task HandleAsync_WhereIbisCustomerReferenceNumberIsChangingToBlankAndBlankAlreadyExists_ReturnsSuccessResult()
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

            Scheme otherScheme = new Scheme(A.Dummy<Organisation>());
            typeof(Entity).GetProperty("Id").SetValue(otherScheme, new Guid("C78D98A9-E33A-4E20-88D3-F1C99D5165B1"));
            otherScheme.UpdateScheme(
                "Scheme 2",
                "WEE/BB1111BB/SCH",
                string.Empty,
                Domain.Obligation.ObligationType.B2C,
                environmentAgency);
            otherScheme.SetStatus(Domain.Scheme.SchemeStatus.Approved);

            IUpdateSchemeInformationDataAccess dataAccess = A.Fake<IUpdateSchemeInformationDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(new Guid("5AE25C37-88C8-4646-8793-DB4C2F4EF0E5"))).Returns(scheme);
            A.CallTo(() => dataAccess.FetchEnvironmentAgencyAsync()).Returns(environmentAgency);
            A.CallTo(() => dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync())
                .Returns(new List<Scheme>() { otherScheme });

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
            Assert.Equal(UpdateSchemeInformationResult.ResultType.Success, result.Result);
        }
    }
}