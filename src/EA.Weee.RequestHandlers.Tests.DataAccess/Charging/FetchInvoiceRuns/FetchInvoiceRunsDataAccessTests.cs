namespace EA.Weee.RequestHandlers.Tests.DataAccess.Charging.FetchInvoiceRuns
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Charges.FetchInvoiceRuns;
    using Domain;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class FetchInvoiceRunsDataAccessTests
    {
        /// <summary>
        /// This test ensures that the FetchInvoiceRuns data access method only returns invoice runs
        /// for the specified authority.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchInvoiceRunsAsync_WithSpecifiedAuthority_OnlyReturnsInvoiceRunsForTheSpecifiedAuthority()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                AspNetUser user = helper.GetOrCreateUser("TestUser");

                Weee.Tests.Core.Model.Country country = new Weee.Tests.Core.Model.Country();
                country.Id = new Guid("FA20ED45-5488-491D-A117-DFC09C9C1BA2");
                country.Name = "Test Country";

                CompetentAuthority databaseAuthority1 = new CompetentAuthority();
                databaseAuthority1.Id = new Guid("DDE398F6-809E-416D-B70D-B36606F221FC");
                databaseAuthority1.Name = "Test Authority 1";
                databaseAuthority1.Abbreviation = "T1";
                databaseAuthority1.Country = country;
                databaseAuthority1.Email = "TestEmailAddress";
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority1);

                CompetentAuthority databaseAuthority2 = new CompetentAuthority();
                databaseAuthority2.Id = new Guid("FBCEDC2F-0825-4066-B24E-86D3A2FD892B");
                databaseAuthority2.Name = "Test Authority 2";
                databaseAuthority2.Abbreviation = "T2";
                databaseAuthority2.Country = country;
                databaseAuthority2.Email = "TestEmailAddress2";
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority2);

                InvoiceRun invoiceRunForAuthority1 = new InvoiceRun();
                invoiceRunForAuthority1.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRunForAuthority1.CompetentAuthority = databaseAuthority1;
                invoiceRunForAuthority1.IssuedByUserId = user.Id;
                invoiceRunForAuthority1.IssuedDate = new DateTime(2015, 1, 1);
                wrapper.Model.InvoiceRuns.Add(invoiceRunForAuthority1);

                InvoiceRun invoiceRunForAuthority2 = new InvoiceRun();
                invoiceRunForAuthority2.Id = new Guid("728CDF55-1C3C-4BE0-80CB-0BC82CC9DFA3");
                invoiceRunForAuthority2.CompetentAuthority = databaseAuthority2;
                invoiceRunForAuthority2.IssuedByUserId = user.Id;
                invoiceRunForAuthority2.IssuedDate = new DateTime(2015, 1, 1);
                wrapper.Model.InvoiceRuns.Add(invoiceRunForAuthority2);

                wrapper.Model.SaveChanges();

                UKCompetentAuthority domainAuthority1 = wrapper.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority1.Id);

                FetchInvoiceRunsDataAccess dataAccess = new FetchInvoiceRunsDataAccess(wrapper.WeeeContext);

                // Act
                IReadOnlyList<Domain.Charges.InvoiceRun> results = await dataAccess.FetchInvoiceRunsAsync(domainAuthority1);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                Assert.Equal(new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872"), results[0].Id);
            }
        }

        /// <summary>
        /// This test ensures that the FetchInvoiceRuns data access method returns invoice runs
        /// in order of their issued date descending.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchInvoiceRunsAsync_Always_ReturnsResultsOrderedByIssuedDateDescending()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                AspNetUser user = helper.GetOrCreateUser("TestUser");

                Weee.Tests.Core.Model.Country country = new Weee.Tests.Core.Model.Country();
                country.Id = new Guid("FA20ED45-5488-491D-A117-DFC09C9C1BA2");
                country.Name = "Test Country";

                CompetentAuthority databaseAuthority = new CompetentAuthority();
                databaseAuthority.Id = new Guid("DDE398F6-809E-416D-B70D-B36606F221FC");
                databaseAuthority.Name = "Test Authority 1";
                databaseAuthority.Abbreviation = "T1";
                databaseAuthority.Country = country;
                databaseAuthority.Email = "TestEmailAddress";
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority);

                InvoiceRun invoiceRun1 = new InvoiceRun();
                invoiceRun1.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRun1.CompetentAuthority = databaseAuthority;
                invoiceRun1.IssuedByUserId = user.Id;
                invoiceRun1.IssuedDate = new DateTime(2015, 1, 2);
                wrapper.Model.InvoiceRuns.Add(invoiceRun1);

                InvoiceRun invoiceRun2 = new InvoiceRun();
                invoiceRun2.Id = new Guid("728CDF55-1C3C-4BE0-80CB-0BC82CC9DFA3");
                invoiceRun2.CompetentAuthority = databaseAuthority;
                invoiceRun2.IssuedByUserId = user.Id;
                invoiceRun2.IssuedDate = new DateTime(2015, 1, 1);
                wrapper.Model.InvoiceRuns.Add(invoiceRun2);

                InvoiceRun invoiceRun3 = new InvoiceRun();
                invoiceRun3.Id = new Guid("B235CD4F-8188-4BC0-ADA2-55FD6B34BC01");
                invoiceRun3.CompetentAuthority = databaseAuthority;
                invoiceRun3.IssuedByUserId = user.Id;
                invoiceRun3.IssuedDate = new DateTime(2015, 1, 3);
                wrapper.Model.InvoiceRuns.Add(invoiceRun3);

                wrapper.Model.SaveChanges();

                UKCompetentAuthority domainAuthority = wrapper.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority.Id);

                FetchInvoiceRunsDataAccess dataAccess = new FetchInvoiceRunsDataAccess(wrapper.WeeeContext);

                // Act
                IReadOnlyList<Domain.Charges.InvoiceRun> results = await dataAccess.FetchInvoiceRunsAsync(domainAuthority);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(3, results.Count);
                Assert.Collection(results,
                    r1 => Assert.Equal(new DateTime(2015, 1, 3), r1.IssuedDate),
                    r2 => Assert.Equal(new DateTime(2015, 1, 2), r2.IssuedDate),
                    r3 => Assert.Equal(new DateTime(2015, 1, 1), r3.IssuedDate));
            }
        }

        /// <summary>
        /// This test ensures that the FetchInvoiceRuns data access method returns invoice runs
        /// with the IssuedByUser eagerly loaded.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchInvoiceRunsAsync_Always_EagerLoadsIssuedByUser()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                AspNetUser user = helper.GetOrCreateUser("TestUser");

                Weee.Tests.Core.Model.Country country = new Weee.Tests.Core.Model.Country();
                country.Id = new Guid("FA20ED45-5488-491D-A117-DFC09C9C1BA2");
                country.Name = "Test Country";

                CompetentAuthority databaseAuthority = new CompetentAuthority();
                databaseAuthority.Id = new Guid("DDE398F6-809E-416D-B70D-B36606F221FC");
                databaseAuthority.Name = "Test Authority 1";
                databaseAuthority.Abbreviation = "T1";
                databaseAuthority.Country = country;
                databaseAuthority.Email = "TestEmailAddress";
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority);

                InvoiceRun invoiceRun = new InvoiceRun();
                invoiceRun.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRun.CompetentAuthority = databaseAuthority;
                invoiceRun.IssuedByUserId = user.Id;
                invoiceRun.IssuedDate = new DateTime(2015, 1, 2);
                wrapper.Model.InvoiceRuns.Add(invoiceRun);

                wrapper.Model.SaveChanges();

                UKCompetentAuthority domainAuthority = wrapper.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority.Id);

                FetchInvoiceRunsDataAccess dataAccess = new FetchInvoiceRunsDataAccess(wrapper.WeeeContext);

                // Act
                IReadOnlyList<Domain.Charges.InvoiceRun> results = await dataAccess.FetchInvoiceRunsAsync(domainAuthority);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                Assert.NotNull(results[0].IssuedByUser);
            }
        }
    }
}
