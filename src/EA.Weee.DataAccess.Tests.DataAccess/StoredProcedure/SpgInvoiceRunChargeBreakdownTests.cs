namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgInvoiceRunChargeBreakdownTests
    {
        [Fact]
        public async Task Execute_ReturnsProducers_ForSpecifiedInvoiceRunOnly()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var invoiceRun1 = helper.CreateInvoiceRun();

                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme, invoiceRun1);
                memberUpload1.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer1.ChargeThisUpdate = 10;
                producer1.Invoiced = true;

                var invoiceRun2 = helper.CreateInvoiceRun();

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme, invoiceRun2);
                memberUpload2.ComplianceYear = 2016;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN567");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun2.Id);

                Assert.Single(result);
                Assert.Equal("PRN567", result.Single().PRN);
            }
        }

        [Fact]
        public async Task Execute_ReturnsInvoicedProducers_Only()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun);
                memberUpload.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                producer1.ChargeThisUpdate = 10;
                producer1.Invoiced = true;

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN567");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = false;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("PRN123", result.Single().PRN);
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducers_WithNonZeroChargesOnly()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun);
                memberUpload.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                producer1.ChargeThisUpdate = 0;
                producer1.Invoiced = true;

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN567");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("PRN567", result.Single().PRN);
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducers_OrderedBy_SchemeAscending()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var invoiceRun = helper.CreateInvoiceRun();

                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "BBB";

                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload1.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer1.ChargeThisUpdate = 10;
                producer1.Invoiced = true;

                var scheme2 = helper.CreateScheme();
                scheme2.SchemeName = "AAA";

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2, invoiceRun);
                memberUpload2.ComplianceYear = 2016;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN567");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = true;

                var scheme3 = helper.CreateScheme();
                scheme3.SchemeName = "CCC";

                var memberUpload3 = helper.CreateSubmittedMemberUpload(scheme3, invoiceRun);
                memberUpload3.ComplianceYear = 2016;

                var producer3 = helper.CreateProducerAsCompany(memberUpload3, "PRN987");
                producer3.ChargeThisUpdate = 10;
                producer3.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Collection(result,
                    r1 => Assert.Equal("PRN567", r1.PRN),
                    r2 => Assert.Equal("PRN123", r2.PRN),
                    r3 => Assert.Equal("PRN987", r3.PRN));
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducers_OrderedBy_SchemeAscending_ThenByComplianceYearDescending()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var invoiceRun = helper.CreateInvoiceRun();

                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "BBB";

                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload1.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer1.ChargeThisUpdate = 10;
                producer1.Invoiced = true;

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload2.ComplianceYear = 2015;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN987");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = true;

                var scheme2 = helper.CreateScheme();
                scheme2.SchemeName = "AAA";

                var memberUpload3 = helper.CreateSubmittedMemberUpload(scheme2, invoiceRun);
                memberUpload3.ComplianceYear = 2015;

                var producer3 = helper.CreateProducerAsCompany(memberUpload3, "PRN567");
                producer3.ChargeThisUpdate = 10;
                producer3.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Collection(result,
                    r1 => Assert.Equal("PRN567", r1.PRN),
                    r2 => Assert.Equal("PRN123", r2.PRN),
                    r3 => Assert.Equal("PRN987", r3.PRN));
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducers_OrderedBy_SchemeAscending_ThenByComplianceYearDescending_ThenBySubmittedDateAscending()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var invoiceRun = helper.CreateInvoiceRun();

                var scheme1 = helper.CreateScheme();
                scheme1.SchemeName = "BBB";

                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.SubmittedDate = new DateTime(2016, 12, 31);

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer1.ChargeThisUpdate = 10;
                producer1.Invoiced = true;

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.SubmittedDate = new DateTime(2015, 12, 31);

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN987");
                producer2.ChargeThisUpdate = 10;
                producer2.Invoiced = true;

                var memberUpload3 = helper.CreateSubmittedMemberUpload(scheme1, invoiceRun);
                memberUpload3.ComplianceYear = 2015;
                memberUpload3.SubmittedDate = new DateTime(2015, 01, 01);

                var producer3 = helper.CreateProducerAsCompany(memberUpload3, "PRN999");
                producer3.ChargeThisUpdate = 10;
                producer3.Invoiced = true;

                var scheme2 = helper.CreateScheme();
                scheme2.SchemeName = "AAA";

                var memberUpload4 = helper.CreateSubmittedMemberUpload(scheme2, invoiceRun);
                memberUpload4.ComplianceYear = 2017;
                memberUpload4.SubmittedDate = new DateTime(2017, 12, 31);

                var producer4 = helper.CreateProducerAsCompany(memberUpload4, "PRN567");
                producer4.ChargeThisUpdate = 10;
                producer4.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Collection(result,
                    r1 => Assert.Equal("PRN567", r1.PRN),
                    r2 => Assert.Equal("PRN123", r2.PRN),
                    r3 => Assert.Equal("PRN999", r3.PRN),
                    r4 => Assert.Equal("PRN987", r4.PRN));
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducer_CountryForPartnerShip()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun);
                memberUpload.ComplianceYear = 2019;

                var producer = helper.CreateProducerAsPartnership(memberUpload, "PRN567");
                producer.ChargeThisUpdate = 10;
                producer.Invoiced = true;
                producer.Business.Partnership.Contact1.Address1.Country.Name = "FRANCE";

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("FRANCE", result.Single().RegOfficeOrPBoBCountry);
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducer_CountryForCompany()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun);
                memberUpload.ComplianceYear = 2019;

                var producer = helper.CreateProducerAsCompany(memberUpload, "PRN567");
                producer.ChargeThisUpdate = 10;
                producer.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("UK - England", result.Single().RegOfficeOrPBoBCountry);
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducer_HasAnnualCharge_ReturnsNo()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();
                
                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun);
                memberUpload.ComplianceYear = 2019;

                var producer = helper.CreateProducerAsCompany(memberUpload, "PRN567");
                producer.ChargeThisUpdate = 10;
               
                producer.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("No", result.Single().HasAnnualCharge);
            }
        }

        [Fact]
        public async Task Execute_ReturnsProducer_HasAnnualCharge_ReturnsYes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(db.Model);

                var organisation = helper.CreateOrganisation();
               
                Scheme scheme = helper.CreateScheme();

                scheme.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme.SchemeName = "SchemeName";

                scheme.Organisation.Name = "Org Annual Charge Test";
                scheme.CompetentAuthorityId = new Guid("a3c2d0dd-53a1-4f6a-99d0-1ccfc87611a8");
                scheme.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme.Organisation.Address = helper.CreateOrganisationAddress();
                scheme.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme.Organisation.Address.CountryId = new Guid("a3c2d0dd-53a1-4f6a-99d0-1ccfc87611a8");

                var eascheme = scheme.Organisation.Address.Country.CompetentAuthorities.FirstOrDefault(c => c.Abbreviation == "EA");
                eascheme.AnnualChargeAmount = 100;

                var invoiceRun = helper.CreateInvoiceRun();

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRun, true);
                memberUpload.ComplianceYear = 2019;

                var producer = helper.CreateProducerAsCompany(memberUpload, "PRN567");
                producer.ChargeBandAmount = helper.FetchChargeBandAmount(Domain.Lookup.ChargeBand.E);
                producer.ChargeThisUpdate = 10;
                producer.Invoiced = true;

                db.Model.SaveChanges();

                var result = await db.StoredProcedures.SpgInvoiceRunChargeBreakdown(invoiceRun.Id);

                Assert.Single(result);
                Assert.Equal("Yes", result.Single().HasAnnualCharge);
            }
        }
    }
}
