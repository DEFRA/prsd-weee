namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain;
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    public class SubmitSchemeObligationRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenISubmitSchemeObligationWithNoErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                schemes.Add(SchemeDbSetup.Init().Create());
                schemes.Add(SchemeDbSetup.Init().Create());

                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {schemes.ElementAt(0).ApprovalNumber},{schemes.ElementAt(0).SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,
                {schemes.ElementAt(1).ApprovalNumber},{schemes.ElementAt(1).SchemeName}, ,15,16,17,18,19,20,21,22,23,24,25,26,27";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(0);
            };

            private readonly It shouldHaveCreatedObligationSchemes = () =>
            {
                var schemeObligation = obligationUpload.ObligationSchemes.First(s => s.Scheme.Id == schemes.ElementAt(0).Id);
                schemeObligation.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(15));

                var schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(1);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(2);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment);
                schemeCategoryObligation.Obligation.Should().Be(3);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment);
                schemeCategoryObligation.Obligation.Should().Be(4);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LightingEquipment);
                schemeCategoryObligation.Obligation.Should().Be(5);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools);
                schemeCategoryObligation.Obligation.Should().Be(6);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports);
                schemeCategoryObligation.Obligation.Should().Be(7);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MedicalDevices);
                schemeCategoryObligation.Obligation.Should().Be(8);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments);
                schemeCategoryObligation.Obligation.Should().Be(9);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers);
                schemeCategoryObligation.Obligation.Should().Be(10);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.DisplayEquipment);
                schemeCategoryObligation.Obligation.Should().Be(11);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeCategoryObligation.Obligation.Should().Be(12);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources);
                schemeCategoryObligation.Obligation.Should().Be(13);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels);
                schemeCategoryObligation.Obligation.Should().BeNull();

                schemeObligation = obligationUpload.ObligationSchemes.First(s => s.Scheme.Id == schemes.ElementAt(1).Id);
                schemeObligation.ComplianceYear.Should().Be(2022);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(15));
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().BeNull();
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(15);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment);
                schemeCategoryObligation.Obligation.Should().Be(16);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment);
                schemeCategoryObligation.Obligation.Should().Be(17);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LightingEquipment);
                schemeCategoryObligation.Obligation.Should().Be(18);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools);
                schemeCategoryObligation.Obligation.Should().Be(19);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports);
                schemeCategoryObligation.Obligation.Should().Be(20);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MedicalDevices);
                schemeCategoryObligation.Obligation.Should().Be(21);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments);
                schemeCategoryObligation.Obligation.Should().Be(22);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers);
                schemeCategoryObligation.Obligation.Should().Be(23);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.DisplayEquipment);
                schemeCategoryObligation.Obligation.Should().Be(24);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeCategoryObligation.Obligation.Should().Be(25);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources);
                schemeCategoryObligation.Obligation.Should().Be(26);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels);
                schemeCategoryObligation.Obligation.Should().Be(27);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithNoErrorsAndUpdates : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private static List<ObligationScheme> obligationSchemes;

            private readonly Establish context = () =>
            {
                LocalSetup();
                obligationSchemes = new List<ObligationScheme>();
                schemes.Add(SchemeDbSetup.Init().Create());
                schemes.Add(SchemeDbSetup.Init().Create());

                var amounts1 = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 1000),
                    new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, null),
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, null),
                    new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, null),
                    new ObligationSchemeAmount(WeeeCategory.LightingEquipment, 2000),
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, null),
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, null),
                    new ObligationSchemeAmount(WeeeCategory.MedicalDevices, null),
                    new ObligationSchemeAmount(WeeeCategory.MonitoringAndControlInstruments, null),
                    new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 3000),
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, null),
                    new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, null),
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, null),
                    new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 4000)
                };

                obligationUpload = ObligationUploadDbSetup.Init().Create();

                var obligationScheme1 = ObligationSchemeDbSetup
                    .Init()
                    .WithScheme(schemes.ElementAt(0).Id)
                    .WithObligationAmounts(amounts1)
                    .WithObligationUpload(obligationUpload.Id)
                    .Create();

                obligationSchemes.Add(obligationScheme1);
                
                var amounts2 = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 1000),
                    new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, 2000),
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 3000),
                    new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 4000),
                    new ObligationSchemeAmount(WeeeCategory.LightingEquipment, 5000),
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, 6000),
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, 7000),
                    new ObligationSchemeAmount(WeeeCategory.MedicalDevices, 8000),
                    new ObligationSchemeAmount(WeeeCategory.MonitoringAndControlInstruments, 9000),
                    new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 10000),
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, 11000),
                    new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, 12000),
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 13000),
                    new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 14000)
                };

                var obligationScheme2 = ObligationSchemeDbSetup
                    .Init()
                    .WithScheme(schemes.ElementAt(1).Id)
                    .WithObligationAmounts(amounts2)
                    .WithObligationUpload(obligationUpload.Id)
                    .Create();

                obligationSchemes.Add(obligationScheme2);

                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {schemes.ElementAt(0).ApprovalNumber},{schemes.ElementAt(0).SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,
                {schemes.ElementAt(1).ApprovalNumber},{schemes.ElementAt(1).SchemeName}, ,15,16,17,18,19,20,21,22,23,24,25,26,27";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(0);
            };

            private readonly It shouldHaveUpdatedObligationScheme = () =>
            {
                foreach (var obligationScheme in obligationSchemes)
                {
                    var scheme = Query.GetSchemeById(obligationScheme.SchemeId);

                    scheme.ObligationSchemes.First(os => os.ComplianceYear == SystemTime.UtcNow.Year).ObligationUploadId.Should()
                        .Be(obligationUpload.Id);
                }
            };

            private readonly It shouldHaveUpdatedObligationSchemes = () =>
            {
                var schemeObligation = obligationUpload.ObligationSchemes.First(s => s.Scheme.Id == schemes.ElementAt(0).Id);
                schemeObligation.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));

                var schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(1);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(2);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment);
                schemeCategoryObligation.Obligation.Should().Be(3);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment);
                schemeCategoryObligation.Obligation.Should().Be(4);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LightingEquipment);
                schemeCategoryObligation.Obligation.Should().Be(5);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools);
                schemeCategoryObligation.Obligation.Should().Be(6);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports);
                schemeCategoryObligation.Obligation.Should().Be(7);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MedicalDevices);
                schemeCategoryObligation.Obligation.Should().Be(8);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments);
                schemeCategoryObligation.Obligation.Should().Be(9);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers);
                schemeCategoryObligation.Obligation.Should().Be(10);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.DisplayEquipment);
                schemeCategoryObligation.Obligation.Should().Be(11);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeCategoryObligation.Obligation.Should().Be(12);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources);
                schemeCategoryObligation.Obligation.Should().Be(13);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels);
                schemeCategoryObligation.Obligation.Should().BeNull();

                schemeObligation = obligationUpload.ObligationSchemes.First(s => s.Scheme.Id == schemes.ElementAt(1).Id);
                schemeObligation.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);
                schemeObligation.UpdatedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().BeNull();
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(15);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment);
                schemeCategoryObligation.Obligation.Should().Be(16);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment);
                schemeCategoryObligation.Obligation.Should().Be(17);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LightingEquipment);
                schemeCategoryObligation.Obligation.Should().Be(18);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools);
                schemeCategoryObligation.Obligation.Should().Be(19);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports);
                schemeCategoryObligation.Obligation.Should().Be(20);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MedicalDevices);
                schemeCategoryObligation.Obligation.Should().Be(21);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments);
                schemeCategoryObligation.Obligation.Should().Be(22);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers);
                schemeCategoryObligation.Obligation.Should().Be(23);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.DisplayEquipment);
                schemeCategoryObligation.Obligation.Should().Be(24);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeCategoryObligation.Obligation.Should().Be(25);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources);
                schemeCategoryObligation.Obligation.Should().Be(26);
                schemeCategoryObligation =
                    schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels);
                schemeCategoryObligation.Obligation.Should().Be(27);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithDataErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();

                schemes.Add(scheme1);
                schemes.Add(scheme2);
                
                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},Invalid,2,3,4,5,6,7,8,9,10,11,12,13,14
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,190000000000000,20,21,22,23,24,25,26,";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();

                obligationUpload.ObligationUploadErrors.Count.Should().Be(2);

                obligationUpload.ObligationUploadErrors.Should().Contain(e => e.SchemeIdentifier.Equals(schemes.ElementAt(0).ApprovalNumber)
                                                                              && e.SchemeName.Equals(schemes.ElementAt(0).SchemeName) && 
                                                                              e.ErrorType.Equals(ObligationUploadErrorType.Data));

                obligationUpload.ObligationUploadErrors.Should().Contain(e => e.SchemeIdentifier.Equals(schemes.ElementAt(1).ApprovalNumber)
                                                                              && e.SchemeName.Equals(schemes.ElementAt(1).SchemeName) &&
                                                                              e.ErrorType.Equals(ObligationUploadErrorType.Data));
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithFileErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();

                //header incorrect
                var csvHeader =
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t),
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14
                {scheme2.ApprovalNumber},{scheme2.SchemeName},14,15,16,17,18,19,20,21,22,23,24,25,26,";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithIncorrectFileExtension : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();

                schemes.Add(SchemeDbSetup.Init().Create());
                schemes.Add(SchemeDbSetup.Init().Create());

                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {schemes.ElementAt(0).ApprovalNumber},{schemes.ElementAt(0).SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,";

                var fileInfo = new FileInfo("File.txt", Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithIncorrectNumberOfColumns : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();

                //header incorrect
                var csvHeader =
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t),additional
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithIncorrectNumberOfDataColumns : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();

                //header incorrect
                var csvHeader =
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14,additional";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }
        
        [Component]
        public class WhenISubmitSchemeObligationWithIncorrectColumnsOrder : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();
                var scheme2 = SchemeDbSetup.Init().Create();

                //header incorrect
                var csvHeader =
                    $@",Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat14 (t),Cat13 (t)
                {scheme1.ApprovalNumber},{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,14";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.File);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        [Component]
        public class WhenISubmitSchemeObligationWithSchemeErrors : SubmitSchemeObligationHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var scheme1 = SchemeDbSetup.Init().Create();

                //header incorrect
                var csvHeader =
                    $@"Scheme Identifier,Scheme Name,Cat1 (t),Cat2 (t),Cat3 (t),Cat4 (t),Cat5 (t),Cat6 (t),Cat7 (t),Cat8 (t),Cat9 (t),Cat10 (t),Cat11 (t),Cat12 (t),Cat13 (t),Cat14 (t)
                {scheme1.ApprovalNumber}nomatch,{scheme1.SchemeName},1,2,3,4,5,6,7,8,9,10,11,12,13,";

                var fileInfo = new FileInfo(FileName, Encoding.UTF8.GetBytes(csvHeader));
                request = new SubmitSchemeObligation(fileInfo, CompetentAuthority.England, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                obligationUpload = Query.GetObligationUploadById(result);
            };

            private readonly It shouldHaveCreatedUpload = () =>
            {
                MapStandardProperties();
                obligationUpload.ObligationUploadErrors.Count.Should().Be(1);
                obligationUpload.ObligationUploadErrors.ElementAt(0).ErrorType.Should()
                    .Be(ObligationUploadErrorType.Scheme);
            };

            private readonly Cleanup cleanup = LocalCleanup;
        }

        public class SubmitSchemeObligationHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<SubmitSchemeObligation, Guid> handler;
            protected static SubmitSchemeObligation request;
            protected static ObligationUpload obligationUpload;
            protected static UKCompetentAuthority authority;
            protected static List<Scheme> schemes;
            protected static Guid result;
            protected static Fixture fixture;
            protected const string FileName = "File.csv";

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);
                authority = Query.GetEaCompetentAuthority();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<SubmitSchemeObligation, Guid>>();
                schemes = new List<Scheme>();
                return setup;
            }
            protected static void LocalCleanup()
            {
                obligationUpload = null;
                schemes = null;
            }

            protected static void MapStandardProperties()
            {
                obligationUpload.Should().NotBeNull();
                obligationUpload.Data.Should().Be(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data));
                obligationUpload.FileName.Should().Be(request.FileInfo.FileName);
                obligationUpload.UploadedById.Should().Be(UserId.ToString());
                obligationUpload.CompetentAuthorityId.Should().Be(authority.Id);
                obligationUpload.UploadedDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(20));
            }
        }
    }
}
