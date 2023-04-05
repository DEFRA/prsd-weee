namespace EA.Weee.DataAccess.Tests.Integration.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Error;
    using Domain.Evidence;
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

                var obligatedUpload =
                    new ObligationUpload(matchingAuthority, context.GetCurrentUser(), "data", "filename");
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

                var pendingSchemeMatchingAuthorityWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingSchemeMatchingAuthorityWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, pendingSchemeMatchingAuthorityWithData);
                var obligationScheme1 = new ObligationScheme(pendingSchemeMatchingAuthorityWithData, 2022);
                obligationScheme1.UpdateObligationUpload(obligatedUpload);
                pendingSchemeMatchingAuthorityWithData.ObligationSchemes.Add(obligationScheme1);

                var rejectedSchemeMatchingAuthorityWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedSchemeMatchingAuthorityWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, rejectedSchemeMatchingAuthorityWithData);
                var obligationScheme2 = new ObligationScheme(rejectedSchemeMatchingAuthorityWithData, 2022);
                obligationScheme2.UpdateObligationUpload(obligatedUpload);
                rejectedSchemeMatchingAuthorityWithData.ObligationSchemes.Add(obligationScheme2);

                var withdrawnSchemeMatchingAuthorityWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnSchemeMatchingAuthorityWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, withdrawnSchemeMatchingAuthorityWithData);
                var obligationScheme3 = new ObligationScheme(withdrawnSchemeMatchingAuthorityWithData, 2022);
                obligationScheme3.UpdateObligationUpload(obligatedUpload);
                withdrawnSchemeMatchingAuthorityWithData.ObligationSchemes.Add(obligationScheme3);

                var pendingSchemeMatchingAuthorityWithDataNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingSchemeMatchingAuthorityWithDataNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, pendingSchemeMatchingAuthorityWithDataNotInComplianceYear);
                var obligationScheme4NotInComplianceYear = new ObligationScheme(pendingSchemeMatchingAuthorityWithDataNotInComplianceYear, 2023);
                obligationScheme4NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                pendingSchemeMatchingAuthorityWithDataNotInComplianceYear.ObligationSchemes.Add(obligationScheme4NotInComplianceYear);

                var rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear);
                var obligationScheme5NotInComplianceYear = new ObligationScheme(rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear, 2021);
                obligationScheme5NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear.ObligationSchemes.Add(obligationScheme5NotInComplianceYear);

                var withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    matchingAuthority, withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear);
                var obligationScheme6NotInComplianceYear = new ObligationScheme(withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear, 2019);
                obligationScheme6NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear.ObligationSchemes.Add(obligationScheme6NotInComplianceYear);

                context.Schemes.Add(rejectedSchemeMatchingAuthority);
                context.Schemes.Add(pendingSchemeMatchingAuthority);
                context.Schemes.Add(withdrawnSchemeMatchingAuthority);
                context.Schemes.Add(matchingSchemeMatchingAuthority);
                context.Schemes.Add(nonMatchingAuthorityScheme);
                context.Schemes.Add(pendingSchemeMatchingAuthorityWithData);
                context.Schemes.Add(rejectedSchemeMatchingAuthorityWithData);
                context.Schemes.Add(withdrawnSchemeMatchingAuthorityWithData);
                context.Schemes.Add(pendingSchemeMatchingAuthorityWithDataNotInComplianceYear);
                context.Schemes.Add(rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear);
                context.Schemes.Add(withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationSchemeData(matchingAuthority, 2022);

                results.Should().AllSatisfy(s =>
                {
                    s.CompetentAuthority.Id.Should().Be(matchingAuthority.Id);
                });

                results.Should().NotContain(s => s.Id == pendingSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.Id == rejectedSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.Id == withdrawnSchemeMatchingAuthority.Id);
                results.Should().NotContain(s => s.Id == nonMatchingAuthorityScheme.Id);
                results.Should().NotContain(s => s.CompetentAuthority.Id != matchingAuthority.Id);
                results.Should().Contain(s => s.Id == pendingSchemeMatchingAuthorityWithData.Id);
                results.Should().Contain(s => s.Id == rejectedSchemeMatchingAuthorityWithData.Id);
                results.Should().Contain(s => s.Id == withdrawnSchemeMatchingAuthorityWithData.Id);
                results.Should().NotContain(s => s.Id == pendingSchemeMatchingAuthorityWithDataNotInComplianceYear.Id);
                results.Should().NotContain(s => s.Id == rejectedSchemeMatchingAuthorityWithDataNotInComplianceYear.Id);
                results.Should().NotContain(s => s.Id == withdrawnSchemeMatchingAuthorityWithDataNotInComplianceYear.Id);
            }
        }

        [Fact]
        public async Task GetObligationScheme_GivenNoAuthorityAndComplianceYear_ValidSchemesShouldBeReturned()
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

                var authority1 = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var authority2 =
                    await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.NorthernIreland);

                var obligatedUpload =
                    new ObligationUpload(authority1, context.GetCurrentUser(), "data", "filename");
                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                var pendingScheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingScheme);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, pendingScheme);

                var rejectedScheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedScheme);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, rejectedScheme);

                var withdrawnScheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnScheme);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, withdrawnScheme);

                var matchingSchemeApproved = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Approved,
                    matchingSchemeApproved);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, matchingSchemeApproved);

                var alternativeMatchingSchemeApproved = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Approved,
                    alternativeMatchingSchemeApproved);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority2, alternativeMatchingSchemeApproved);

                var pendingSchemeWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingSchemeWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, pendingSchemeWithData);
                var obligationScheme1 = new ObligationScheme(pendingSchemeWithData, 2022);
                obligationScheme1.UpdateObligationUpload(obligatedUpload);
                pendingSchemeWithData.ObligationSchemes.Add(obligationScheme1);

                var rejectedSchemeWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedSchemeWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, rejectedSchemeWithData);
                var obligationScheme2 = new ObligationScheme(rejectedSchemeWithData, 2022);
                obligationScheme2.UpdateObligationUpload(obligatedUpload);
                rejectedSchemeWithData.ObligationSchemes.Add(obligationScheme2);

                var withdrawnSchemeWithData = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnSchemeWithData);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, withdrawnSchemeWithData);
                var obligationScheme3 = new ObligationScheme(withdrawnSchemeWithData, 2022);
                obligationScheme3.UpdateObligationUpload(obligatedUpload);
                withdrawnSchemeWithData.ObligationSchemes.Add(obligationScheme3);

                var pendingSchemeNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Pending,
                    pendingSchemeNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, pendingSchemeNotInComplianceYear);
                var obligationScheme4NotInComplianceYear = new ObligationScheme(pendingSchemeNotInComplianceYear, 2023);
                obligationScheme4NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                pendingSchemeNotInComplianceYear.ObligationSchemes.Add(obligationScheme4NotInComplianceYear);

                var rejectedSchemeNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus, SchemeStatus.Rejected,
                    rejectedSchemeNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, rejectedSchemeNotInComplianceYear);
                var obligationScheme5NotInComplianceYear = new ObligationScheme(rejectedSchemeNotInComplianceYear, 2021);
                obligationScheme5NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                rejectedSchemeNotInComplianceYear.ObligationSchemes.Add(obligationScheme5NotInComplianceYear);

                var withdrawnSchemeNotInComplianceYear = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.SchemeStatus,
                    SchemeStatus.Withdrawn, withdrawnSchemeNotInComplianceYear);
                ObjectInstantiator<EA.Weee.Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthority,
                    authority1, withdrawnSchemeNotInComplianceYear);
                var obligationScheme6NotInComplianceYear = new ObligationScheme(withdrawnSchemeNotInComplianceYear, 2019);
                obligationScheme6NotInComplianceYear.UpdateObligationUpload(obligatedUpload);
                withdrawnSchemeNotInComplianceYear.ObligationSchemes.Add(obligationScheme6NotInComplianceYear);

                context.Schemes.Add(rejectedScheme);
                context.Schemes.Add(pendingScheme);
                context.Schemes.Add(withdrawnScheme);
                context.Schemes.Add(matchingSchemeApproved);
                context.Schemes.Add(alternativeMatchingSchemeApproved);
                context.Schemes.Add(pendingSchemeWithData);
                context.Schemes.Add(rejectedSchemeWithData);
                context.Schemes.Add(withdrawnSchemeWithData);
                context.Schemes.Add(pendingSchemeNotInComplianceYear);
                context.Schemes.Add(rejectedSchemeNotInComplianceYear);
                context.Schemes.Add(withdrawnSchemeNotInComplianceYear);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationSchemeData(null, 2022);

                results.Should().NotContain(s => s.Id == pendingScheme.Id);
                results.Should().NotContain(s => s.Id == rejectedScheme.Id);
                results.Should().NotContain(s => s.Id == withdrawnScheme.Id);
                results.Should().Contain(s => s.Id == pendingSchemeWithData.Id);
                results.Should().Contain(s => s.Id == rejectedSchemeWithData.Id);
                results.Should().Contain(s => s.Id == withdrawnSchemeWithData.Id);
                results.Should().Contain(s => s.Id == alternativeMatchingSchemeApproved.Id);
                results.Should().NotContain(s => s.Id == pendingSchemeNotInComplianceYear.Id);
                results.Should().NotContain(s => s.Id == rejectedSchemeNotInComplianceYear.Id);
                results.Should().NotContain(s => s.Id == withdrawnSchemeNotInComplianceYear.Id);
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

                var obligationScheme2020 = new ObligationScheme(schemeBelongingToAuthority1, 1);
                var obligationScheme2020Duplicate = new ObligationScheme(schemeBelongingToAuthority2, 1);
                var obligationScheme2021 = new ObligationScheme(schemeBelongingToAuthority1, 2);
                var obligationSchemeNotMatchingAuthority = new ObligationScheme(schemeNotBelongingToAuthority2, 3);

                obligatedUpload.ObligationSchemes.Add(obligationScheme2020);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2021);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2020Duplicate);
                obligatedUpload.ObligationSchemes.Add(obligationSchemeNotMatchingAuthority);

                context.ObligationUploads.Add(obligatedUpload);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationComplianceYears(matchingAuthority);

                results.Should().ContainInOrder(new List<int>() { 2, 1 });
                results.Should().OnlyHaveUniqueItems();
                results.Should().BeInDescendingOrder();
                results.Should().NotContain(3);
            }
        }

        [Fact]
        public async Task GetObligationComplianceYears_GivenNullAuthority_ShouldReturnComplianceYears()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess =
                    new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);

                var authority1 = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var authority2 = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.NorthernIreland);

                var obligatedUpload =
                    new ObligationUpload(authority1, context.GetCurrentUser(), "data", "filename");

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                var schemeBelongingToAuthority1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority1.Id, schemeBelongingToAuthority1);

                var schemeBelongingToAuthority2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority1.Id, schemeBelongingToAuthority2);

                var schemeNotBelongingToAuthority2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority2.Id, schemeNotBelongingToAuthority2);

                var obligationScheme2020 = new ObligationScheme(schemeBelongingToAuthority1, 10);
                var obligationScheme2020Duplicate = new ObligationScheme(schemeBelongingToAuthority2, 10);
                var obligationScheme2021 = new ObligationScheme(schemeBelongingToAuthority1, 11);
                var obligationScheme2022 = new ObligationScheme(schemeNotBelongingToAuthority2, 12);

                obligatedUpload.ObligationSchemes.Add(obligationScheme2020);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2021);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2020Duplicate);
                obligatedUpload.ObligationSchemes.Add(obligationScheme2022);

                context.ObligationUploads.Add(obligatedUpload);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetObligationComplianceYears(null);

                results.Should().ContainInOrder(new List<int>() { 12, 11, 10 });
                results.Should().OnlyHaveUniqueItems();
                results.Should().BeInDescendingOrder();
            }
        }

        [Fact]
        public async Task GetSchemesWithObligationOrEvidence_ShouldReturnSchemes()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess = new ObligationDataAccess(userContext, new GenericDataAccess(database.WeeeContext), context);
                var commonDataAccess = new CommonDataAccess(database.WeeeContext);
                var authority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var obligatedUpload = new ObligationUpload(authority, context.GetCurrentUser(), "data", "filename");

                var defaultOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation3 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation4 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation5 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation6 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation7 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation8 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation9 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation10 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation11 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation12 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation13 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation14 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation15 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation16 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation17 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation18 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation19 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation20 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation21 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation22 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation23 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var organisation24 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                // scheme should be included as has obligation in compliance year
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority.Id, scheme1);

                // scheme should be included as has obligation in compliance year
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority.Id, scheme2);

                // scheme should not be returned as incorrect compliance year
                var scheme3 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation3);
                ObjectInstantiator<Domain.Scheme.Scheme>.SetProperty(s => s.CompetentAuthorityId, authority.Id, scheme3);

                var obligationScheme12020 = new ObligationScheme(scheme1, 2022);
                var obligationScheme22020 = new ObligationScheme(scheme2, 2022);
                var obligationSchemeNotFound = new ObligationScheme(scheme3, 2023);

                obligatedUpload.ObligationSchemes.Add(obligationScheme12020);
                obligatedUpload.ObligationSchemes.Add(obligationScheme22020);
                obligatedUpload.ObligationSchemes.Add(obligationSchemeNotFound);

                context.ObligationUploads.Add(obligatedUpload);

                await context.SaveChangesAsync();

                // scheme should be returned as note will be approved in compliance year 
                var scheme4 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation4);
                // scheme should not be returned as note will be created in an incorrect compliance year
                var scheme5 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation5);
                // scheme should be returned as recipient of transfer note in compliance year
                var scheme6 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation6);
                // scheme should not be returned as recipient of transfer note in incorrect compliance year
                var scheme7 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation7);
                // scheme should be returned as originator of transfer note in compliance year
                var scheme8 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation8);
                // scheme should not be returned as originator of transfer note in incorrect compliance year
                var scheme9 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation9);
                // scheme should not be returned as recipient of draft note in compliance year
                var scheme10 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation10);
                // scheme should not be returned as recipient of submitted note in compliance year
                var scheme11 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation11);
                // scheme should not be returned as recipient of returned note in compliance year
                var scheme12 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation12);
                // scheme should not be returned as recipient of rejected note in compliance year
                var scheme13 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation13);
                // scheme should not be returned as recipient of void note in compliance year
                var scheme14 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation14);
                // scheme should not be returned as recipient of draft transfer note in compliance year
                var scheme15 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation15);
                // scheme should not be returned as recipient of submitted transfer note in compliance year
                var scheme16 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation16);
                // scheme should not be returned as recipient of returned transfer note in compliance year
                var scheme17 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation17);
                // scheme should not be returned as recipient of rejected transfer note in compliance year
                var scheme18 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation18);
                // scheme should not be returned as recipient of void transfer note in compliance year
                var scheme19 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation19);
                // scheme should not be returned as originator of draft transfer note in compliance year
                var scheme20 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation20);
                // scheme should not be returned as originator of submitted transfer note in compliance year
                var scheme21 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation21);
                // scheme should not be returned as originator of returned transfer note in compliance year
                var scheme22 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation22);
                // scheme should not be returned as originator of rejected transfer note in compliance year
                var scheme23 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation22);
                // scheme should not be returned as originator of void transfer note in compliance year
                var scheme24 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation24);

                context.Schemes.Add(scheme4);
                context.Schemes.Add(scheme5);
                context.Schemes.Add(scheme6);
                context.Schemes.Add(scheme7);
                context.Schemes.Add(scheme8);
                context.Schemes.Add(scheme9);
                context.Schemes.Add(scheme10);
                context.Schemes.Add(scheme11);
                context.Schemes.Add(scheme12);
                context.Schemes.Add(scheme13);
                context.Schemes.Add(scheme14);
                context.Schemes.Add(scheme15);
                context.Schemes.Add(scheme16);
                context.Schemes.Add(scheme17);
                context.Schemes.Add(scheme18);
                context.Schemes.Add(scheme19);
                context.Schemes.Add(scheme20);
                context.Schemes.Add(scheme21);
                context.Schemes.Add(scheme22);
                context.Schemes.Add(scheme23);
                context.Schemes.Add(scheme24);

                await context.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(database, defaultOrganisation, organisation1, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note1);

                var note2 = NoteCommon.CreateNote(database, defaultOrganisation, organisation4, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note2);

                var note3 = NoteCommon.CreateNote(database, defaultOrganisation, organisation5, complianceYear: 2023);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note3);

                var note4 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation6, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note4);

                var note5 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation7, complianceYear: 2023);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note5);

                var note6 = NoteCommon.CreateTransferNote(database, organisation8, organisation2, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note6);

                var note7 = NoteCommon.CreateTransferNote(database, organisation9, organisation2, complianceYear: 2023);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Approved, note7);

                var note8 = NoteCommon.CreateNote(database, defaultOrganisation, organisation10, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Draft, note8);
                
                var note9 = NoteCommon.CreateNote(database, defaultOrganisation, organisation11, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Submitted, note9);

                var note10 = NoteCommon.CreateNote(database, defaultOrganisation, organisation12, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Returned, note10);

                var note11 = NoteCommon.CreateNote(database, defaultOrganisation, organisation13, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Rejected, note11);

                var note12 = NoteCommon.CreateNote(database, defaultOrganisation, organisation14, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Void, note12);

                var note13 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation15, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Draft, note13);

                var note14 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation16, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Submitted, note14);

                var note15 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation17, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Returned, note15);

                var note16 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation18, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Rejected, note16);

                var note17 = NoteCommon.CreateTransferNote(database, defaultOrganisation, organisation19, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Void, note17);

                var note18 = NoteCommon.CreateTransferNote(database, organisation20, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Draft, note18);

                var note19 = NoteCommon.CreateTransferNote(database, organisation21, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Submitted, note19);

                var note20 = NoteCommon.CreateTransferNote(database, organisation22, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Returned, note20);

                var note21 = NoteCommon.CreateTransferNote(database, organisation23, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Rejected, note21);

                var note22 = NoteCommon.CreateTransferNote(database, organisation24, complianceYear: 2022);
                ObjectInstantiator<Note>.SetProperty(n => n.Status, NoteStatus.Void, note22);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);
                context.Notes.Add(note4);
                context.Notes.Add(note5);
                context.Notes.Add(note6);
                context.Notes.Add(note7);
                context.Notes.Add(note8);
                context.Notes.Add(note9);
                context.Notes.Add(note10);
                context.Notes.Add(note11);
                context.Notes.Add(note12);
                context.Notes.Add(note13);
                context.Notes.Add(note14);
                context.Notes.Add(note15);
                context.Notes.Add(note16);
                context.Notes.Add(note17);
                context.Notes.Add(note18);
                context.Notes.Add(note19);
                context.Notes.Add(note20);
                context.Notes.Add(note21);
                context.Notes.Add(note22);

                await context.SaveChangesAsync();

                var results = await dataAccess.GetSchemesWithObligationOrEvidence(2022);

                results.Should().OnlyHaveUniqueItems();
                results.Should().Contain(scheme1);
                results.Should().Contain(scheme2);
                results.Should().Contain(scheme4);
                results.Should().Contain(scheme6);
                results.Should().Contain(scheme8);
                results.Should().NotContain(scheme3);
                results.Should().NotContain(scheme5);
                results.Should().NotContain(scheme7);
                results.Should().NotContain(scheme9);
                results.Should().NotContain(scheme10);
                results.Should().NotContain(scheme11);
                results.Should().NotContain(scheme12);
                results.Should().NotContain(scheme13);
                results.Should().NotContain(scheme14);
                results.Should().NotContain(scheme15);
                results.Should().NotContain(scheme16);
                results.Should().NotContain(scheme17);
                results.Should().NotContain(scheme18);
                results.Should().NotContain(scheme19);
                results.Should().NotContain(scheme20);
                results.Should().NotContain(scheme21);
                results.Should().NotContain(scheme22);
                results.Should().NotContain(scheme23);
                results.Should().NotContain(scheme24);

                results.Should().BeInAscendingOrder(r => r.SchemeName);
            }
        }
    }
}
