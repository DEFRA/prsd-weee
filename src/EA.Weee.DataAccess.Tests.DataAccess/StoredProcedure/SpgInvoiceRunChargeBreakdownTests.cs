namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
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
    }
}
