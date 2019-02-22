namespace EA.Weee.RequestHandlers.Tests.DataAccess.Charging.FetchIssuedChargesCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Charges;
    using Charges.FetchIssuedChargesCsv;
    using Domain;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class FetchIssuedChargesCsvDataAccessTests
    {
        [Fact]
        public async Task FetchInvoicedProducerSubmissionsAsync_WithSpecifiedAuthorityYearAndSchemeName_OnlyReturnsProducerSubmissions()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                // At least one user is required in the database.
                var user = helper.GetOrCreateUser("A user");

                Weee.Tests.Core.Model.Country country = new Weee.Tests.Core.Model.Country();
                country.Id = new Guid("FA20ED45-5488-491D-A117-DFC09C9C1BA2");
                country.Name = "Test Country";

                CompetentAuthority databaseAuthority1 = new CompetentAuthority();
                databaseAuthority1.Id = new Guid("DDE398F6-809E-416D-B70D-B36606F221FC");
                databaseAuthority1.Name = "Test Authority 1";
                databaseAuthority1.Abbreviation = "T1";
                databaseAuthority1.Country = country;
                databaseAuthority1.Email = "TestEmailAddress";
                databaseAuthority1.AnnualChargeAmount = 0;
                database.Model.CompetentAuthorities.Add(databaseAuthority1);

                InvoiceRun invoiceRunForAuthority1 = new InvoiceRun();
                invoiceRunForAuthority1.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRunForAuthority1.CompetentAuthority = databaseAuthority1;
                invoiceRunForAuthority1.IssuedByUserId = user.Id;
                invoiceRunForAuthority1.IssuedDate = new DateTime(2016, 12, 1);
                database.Model.InvoiceRuns.Add(invoiceRunForAuthority1);

                var scheme = helper.CreateScheme();
                string registrationNumber = "AAAA";

                scheme.CompetentAuthorityId = databaseAuthority1.Id;
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRunForAuthority1);

                var producerSubmission = helper.CreateInvoicedProducer(memberUpload, registrationNumber);

                database.Model.SaveChanges();
                UKCompetentAuthority domainAuthority1 = database.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority1.Id);
                FetchIssuedChargesCsvDataAccess dataAccess = new FetchIssuedChargesCsvDataAccess(database.WeeeContext);

                // Act
                IEnumerable<Domain.Producer.ProducerSubmission> results = await dataAccess.FetchInvoicedProducerSubmissionsAsync(domainAuthority1, 2016, scheme.SchemeName);
                List<Domain.Producer.ProducerSubmission> producerSubmissionList = results.ToList();

                Domain.Producer.ProducerSubmission producer = results.FirstOrDefault(p => p.RegisteredProducer.ProducerRegistrationNumber == registrationNumber);
                // Assert
                Assert.NotNull(producer);
                Assert.Equal(producerSubmission.RegisteredProducer.ProducerRegistrationNumber, producer.RegisteredProducer.ProducerRegistrationNumber);
                Assert.Equal(1, producerSubmissionList.Count);
            }
        }

        [Fact]
        public async Task FetchInvoicedProducerSubmissionsAsync_ReturnsProducerSubmissionsForRemovedProducer_WhenProducerHasBeenInvoiced()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                // At least one user is required in the database.
                var user = helper.GetOrCreateUser("A user");

                Weee.Tests.Core.Model.Country country = new Weee.Tests.Core.Model.Country();
                country.Id = new Guid("FA20ED45-5488-491D-A117-DFC09C9C1BA2");
                country.Name = "Test Country";

                CompetentAuthority competentAuthority = new CompetentAuthority();
                competentAuthority.Id = new Guid("DDE398F6-809E-416D-B70D-B36606F221FC");
                competentAuthority.Name = "Test Authority 1";
                competentAuthority.Abbreviation = "T1";
                competentAuthority.Country = country;
                competentAuthority.Email = "TestEmailAddress";
                competentAuthority.AnnualChargeAmount = 0;
                database.Model.CompetentAuthorities.Add(competentAuthority);

                InvoiceRun invoiceRunForAuthority = new InvoiceRun();
                invoiceRunForAuthority.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRunForAuthority.CompetentAuthority = competentAuthority;
                invoiceRunForAuthority.IssuedByUserId = user.Id;
                invoiceRunForAuthority.IssuedDate = new DateTime(2000, 12, 1);
                database.Model.InvoiceRuns.Add(invoiceRunForAuthority);

                var scheme = helper.CreateScheme();
                string registrationNumber = "AAAA";

                scheme.CompetentAuthorityId = competentAuthority.Id;
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRunForAuthority);
                memberUpload.ComplianceYear = 2000;

                var producerSubmission = helper.CreateInvoicedProducer(memberUpload, registrationNumber);
                producerSubmission.RegisteredProducer.Removed = true;

                database.Model.SaveChanges();

                UKCompetentAuthority domainAuthority = domainHelper.GetCompetentAuthority(competentAuthority.Id);
                FetchIssuedChargesCsvDataAccess dataAccess = new FetchIssuedChargesCsvDataAccess(database.WeeeContext);

                // Act
                var results = await dataAccess.FetchInvoicedProducerSubmissionsAsync(domainAuthority, 2000, scheme.SchemeName);

                // Assert
                Assert.Equal(1, results.Count());

                var producerResult = results.Single();
                Assert.Equal(producerSubmission.Id, producerResult.Id);
            }
        }
    }
}
