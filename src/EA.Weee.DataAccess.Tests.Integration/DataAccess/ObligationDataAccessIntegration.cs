namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using RequestHandlers.Shared;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;

    public class ObligationDataAccessIntegration
    {
        [Fact]
        public async Task AddObligationUpload_WithNoErrorsAndObligationSchemeData_ShouldAddObligationUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var scheme3 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var complianceYear = 2022;

                var obligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 1));

                var obligatedScheme2 = new ObligationScheme(scheme2, complianceYear);
                obligatedScheme2.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels,
                    2));
                obligatedScheme2.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(
                    WeeeCategory.CoolingApplicancesContainingRefrigerants,
                    null));

                var obligatedScheme3 = new ObligationScheme(scheme3, complianceYear);
                obligatedScheme3.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.MedicalDevices,
                    2.1M));

                var obligationScheme = new List<ObligationScheme>()
                {
                    obligatedScheme1,
                    obligatedScheme2,
                    obligatedScheme3
                };

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName,
                    new List<ObligationUploadError>(), obligationScheme);

                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();

                var schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme1.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).CategoryId.Should()
                    .Be(WeeeCategory.ITAndTelecommsEquipment);
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).Obligation.Should().Be(1);

                schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme2.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).CategoryId.Should()
                    .Be(WeeeCategory.PhotovoltaicPanels);
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).Obligation.Should().Be(2);
                schemeObligation.ObligationSchemeAmounts.ElementAt(1).CategoryId.Should()
                    .Be(WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeObligation.ObligationSchemeAmounts.ElementAt(1).Obligation.Should().BeNull();

                schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme3.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).CategoryId.Should()
                    .Be(WeeeCategory.MedicalDevices);
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).Obligation.Should().Be(2.1M);
            }
        }

        [Fact]
        public async Task
            AddObligationUpload_WithNoErrorsAndObligationSchemeDataWithNullObligationAmounts_ShouldAddObligationUploadWithNoObligationScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

                var complianceYear = 2022;

                var obligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 1));

                var obligatedScheme2 = new ObligationScheme(scheme2, complianceYear);
                obligatedScheme2.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels,
                    null));
                obligatedScheme2.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(
                    WeeeCategory.CoolingApplicancesContainingRefrigerants,
                    null));

                var obligationScheme = new List<ObligationScheme>()
                {
                    obligatedScheme1,
                    obligatedScheme2,
                };

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName,
                    new List<ObligationUploadError>(), obligationScheme);

                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();

                obligation.ObligationSchemes.Count.Should().Be(1);
                var schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme1.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).CategoryId.Should()
                    .Be(WeeeCategory.ITAndTelecommsEquipment);
                schemeObligation.ObligationSchemeAmounts.ElementAt(0).Obligation.Should().Be(1);
            }
        }

        [Fact]
        public async Task
            AddObligationUpload_WithNoErrorsAndUpdateOfObligationSchemeData_ShouldAddObligationUploadAndUpdateSchemeObligation()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var complianceYear = 2022;

                var obligatedUpload =
                    new ObligationUpload(authority, context.GetCurrentUser(), "data", "filename");

                var obligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 1));
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5));
                obligatedScheme1.UpdateObligationUpload(obligatedUpload);

                var obligatedScheme2 = new ObligationScheme(scheme2, complianceYear);
                obligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, null));
                obligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 3));
                obligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, null));
                obligatedScheme2.UpdateObligationUpload(obligatedUpload);

                context.ObligationSchemes.Add(obligatedScheme1);
                context.ObligationSchemes.Add(obligatedScheme2);

                await context.SaveChangesAsync();

                var updateObligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                updateObligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 2));

                var updateObligatedScheme2 = new ObligationScheme(scheme2, complianceYear);
                updateObligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, null));
                updateObligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, 1));

                var obligationScheme = new List<ObligationScheme>()
                {
                    updateObligatedScheme1,
                    updateObligatedScheme2
                };

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName,
                    new List<ObligationUploadError>(), obligationScheme);

                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();

                var schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme1.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationUploadId.Should().Be(id);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.ITAndTelecommsEquipment).Obligation.Should().Be(2);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants).Obligation
                    .Should().Be(5);
                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(2);
                schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme2.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationUploadId.Should().Be(id);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.ToysLeisureAndSports).Obligation.Should().BeNull();
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources).Obligation.Should()
                    .BeNull();
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.ElectricalAndElectronicTools).Obligation.Should()
                    .Be(1);
                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(3);
            }
        }

        [Fact]
        public async Task
            AddObligationUpload_WithNoErrorsAndUpdateOfObligationSchemeDataWithAllObligationAmountsAsNull_ShouldAddObligationUploadAndNotUpdateSchemeObligation()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

                var complianceYear = 2022;
                var date = DateTime.Now;

                var obligatedUpload =
                    new ObligationUpload(authority, context.GetCurrentUser(), "data", "filename");

                var obligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 1));
                obligatedScheme1.UpdateObligationUpload(obligatedUpload);
                ObjectInstantiator<ObligationScheme>.SetProperty(os => os.UpdatedDate, date, obligatedScheme1);

                context.ObligationSchemes.Add(obligatedScheme1);

                await context.SaveChangesAsync();

                var updateObligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                updateObligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, null));
                updateObligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, null));

                var obligationScheme = new List<ObligationScheme>()
                {
                    updateObligatedScheme1,
                };

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName,
                    new List<ObligationUploadError>(), obligationScheme);

                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();

                var schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme1.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeSameDateAs(date);
                schemeObligation.ObligationUploadId.Should().Be(id);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.ITAndTelecommsEquipment).Obligation.Should().Be(1);
                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(1);
            }
        }

        [Fact]
        public async Task
            AddObligationUpload_WithNoErrorsWithUpdateAndAdditionOfObligationSchemeData_ShouldAddObligationUploadAndUpdateSchemeObligation()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                var complianceYear = 2022;

                var obligatedUpload =
                    new ObligationUpload(authority, context.GetCurrentUser(), "data", "filename");

                var obligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                obligatedScheme1.ObligationSchemeAmounts.Add(new ObligationSchemeAmount(WeeeCategory.DisplayEquipment,
                    null));
                obligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, 10));
                obligatedScheme1.UpdateObligationUpload(obligatedUpload);

                context.ObligationSchemes.Add(obligatedScheme1);

                await context.SaveChangesAsync();

                var updateObligatedScheme1 = new ObligationScheme(scheme1, complianceYear);
                updateObligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, 2));
                updateObligatedScheme1.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, 11));

                var newObligatedScheme2 = new ObligationScheme(scheme2, complianceYear);
                newObligatedScheme2.ObligationSchemeAmounts.Add(
                    new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 20));

                var obligationScheme = new List<ObligationScheme>()
                {
                    updateObligatedScheme1,
                    newObligatedScheme2
                };

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName,
                    new List<ObligationUploadError>(), obligationScheme);

                await context.SaveChangesAsync();

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Should().BeEmpty();

                var schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme1.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationUploadId.Should().Be(id);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.DisplayEquipment).Obligation.Should().Be(2);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.ToysLeisureAndSports).Obligation.Should().Be(11);
                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(2);

                schemeObligation = obligation.ObligationSchemes.First(s => s.Scheme.Id == scheme2.Id);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeObligation.ObligationUploadId.Should().Be(id);
                schemeObligation.ObligationSchemeAmounts
                    .First(o => o.CategoryId == WeeeCategory.PhotovoltaicPanels).Obligation.Should().Be(20);
                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(1);
            }
        }

        [Fact]
        public async Task AddObligationUpload_WithErrors_ShouldAddObligationUpload()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var fileData = Faker.Lorem.Paragraph();
                var fileName = Faker.Lorem.GetFirstWord();

                var errors = new List<ObligationUploadError>();
                const string description1 = "description1";
                const string scheme1 = "scheme1";
                const string schemeIdentifier1 = "Identifier1";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.Scheme, scheme1, schemeIdentifier1,
                    description1));

                const string description2 = "description2";
                const string scheme2 = "scheme2";
                const string schemeIdentifier2 = "Identifier2";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.Data, WeeeCategory.ConsumerEquipment,
                    scheme2, schemeIdentifier2, description2));

                const string description3 = "description3";

                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, description3));

                //act
                var id = await dataAccess.AddObligationUpload(authority, fileData, fileName, errors,
                    new List<ObligationScheme>());

                var obligation = await genericDataAccess.GetById<ObligationUpload>(id);

                //assert
                obligation.Should().NotBeNull();
                obligation.Data.Should().Be(fileData);
                obligation.FileName.Should().Be(fileName);
                obligation.CompetentAuthority.Should().Be(authority);
                obligation.UploadedById.Should().Be(context.GetCurrentUser());
                obligation.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(10));
                obligation.ObligationUploadErrors.Count.Should().Be(3);
                obligation.ObligationUploadErrors.FirstOrDefault(e =>
                    e.SchemeIdentifier.Equals(schemeIdentifier1) &&
                    e.SchemeName.Equals(scheme1) &&
                    e.ErrorType.Value == ObligationUploadErrorType.Scheme.Value &&
                    e.Description.Equals(description1)).Should().NotBeNull();
                obligation.ObligationUploadErrors.FirstOrDefault(e => e.SchemeIdentifier.Equals(schemeIdentifier2) &&
                                                                      e.SchemeName.Equals(scheme2) &&
                                                                      e.ErrorType == ObligationUploadErrorType.Data &&
                                                                      e.Description.Equals(description2) &&
                                                                      e.Category == WeeeCategory.ConsumerEquipment)
                    .Should().NotBeNull();
                obligation.ObligationUploadErrors.FirstOrDefault(e => e.SchemeIdentifier == null &&
                                                                      e.SchemeName == null &&
                                                                      e.ErrorType == ObligationUploadErrorType.File &&
                                                                      e.Description.Equals(description3) &&
                                                                      e.Category == null).Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetObligationScheme_GivenAuthorityAndComplianceYear_ValidSchemesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                //arrange
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);

                var matchingAuthority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var nonMatchingAuthority =
                    await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.NorthernIreland);

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                var pendingSchemeMatchingAuthority = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingSchemeMatchingAuthority);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, pendingSchemeMatchingAuthority);

                var rejectedSchemeMatchingAuthority = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedSchemeMatchingAuthority);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, rejectedSchemeMatchingAuthority);

                var withdrawnSchemeMatchingAuthority = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnSchemeMatchingAuthority);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, withdrawnSchemeMatchingAuthority);

                var matchingSchemeMatchingAuthority = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Approved,
                    matchingSchemeMatchingAuthority);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, matchingSchemeMatchingAuthority);

                var nonMatchingAuthorityScheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Approved,
                    nonMatchingAuthorityScheme);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    nonMatchingAuthority, nonMatchingAuthorityScheme);

                context.Schemes.Add(rejectedSchemeMatchingAuthority);
                context.Schemes.Add(pendingSchemeMatchingAuthority);
                context.Schemes.Add(withdrawnSchemeMatchingAuthority);
                context.Schemes.Add(matchingSchemeMatchingAuthority);
                context.Schemes.Add(nonMatchingAuthorityScheme);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationSchemeData(matchingAuthority, 2022);

                results.Should().AllSatisfy(s =>
                {
                    s.SchemeStatus.Value.Should().Be(SchemeStatus.Approved.Value);
                    s.CompetentAuthority.Id.Should().Be(matchingAuthority.Id);
                });

                results.Should().NotContain(s => s.Id == pendingSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.Id == rejectedSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.Id == withdrawnSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.CompetentAuthority.Id != matchingAuthority.Id);
                results.Should().Contain(s => s.Id == matchingSchemeMatchingAuthority.Id);
            }
        }

        [Fact]
        public async Task GetObligationComplianceYears_GivenAuthority_ShouldReturnComplianceYears()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);

                var matchingAuthority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var nonMatchingAuthority =
                    await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.NorthernIreland);

                var obligatedUpload =
                    new ObligationUpload(matchingAuthority, context.GetCurrentUser(), "data", "filename");
                
                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                var schemeBelongingToAuthority1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, matchingAuthority.Id, schemeBelongingToAuthority1);

                var schemeBelongingToAuthority2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, matchingAuthority.Id, schemeBelongingToAuthority2);

                var schemeNotBelongingToAuthority2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, nonMatchingAuthority.Id, schemeNotBelongingToAuthority2);

                var obligationScheme2020 = new ObligationScheme(schemeBelongingToAuthority1, 2020);
                var obligationScheme2020Duplicate = new ObligationScheme(schemeBelongingToAuthority2, 2020);
                var obligationScheme2021 = new ObligationScheme(schemeBelongingToAuthority1, 2021);
                var obligationSchemeNotMatchingAuthority = new ObligationScheme(schemeNotBelongingToAuthority2, 2000);

                obligatedUpload.ObligationSchemes.Add(obligationScheme2020);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2021);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2020Duplicate);

                context.ObligationUploads.Add(obligatedUpload);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationComplianceYears(matchingAuthority);

                results.Should().ContainInOrder(new List<int>() { 2021, 2020 });
                results.Should().OnlyHaveUniqueItems();
                results.Should().BeInDescendingOrder();
                results.Should().NotContain(2000);
            }
        }
    }
}
