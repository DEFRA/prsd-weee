namespace EA.Weee.RequestHandlers.Tests.DataAccess.Charging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Charges;
    using Domain;
    using Weee.Tests.Core.Model;
    using Xunit;
    public class CommonDataAccessTests
    {
        [Fact]
        public async Task FetchSubmittedNonInvoicedMemberUploadsAsync_WithSpecifiedAuthority_OnlyReturnsNonInvoicedMemberUploadsForTheSpecifiedAuthority()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
              
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
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority1);
                
                var scheme = helper.CreateScheme();
                
                scheme.CompetentAuthorityId = databaseAuthority1.Id;             
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);              
                wrapper.Model.SaveChanges();

                UKCompetentAuthority domainAuthority1 = wrapper.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority1.Id);
                CommonDataAccess dataAccess = new CommonDataAccess(wrapper.WeeeContext);

                // Act
                IReadOnlyList<Domain.Scheme.MemberUpload> results = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(domainAuthority1);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                Assert.Equal(memberUpload.Id, results[0].Id);
            }
        }

        [Fact]
        public async Task FetchnvoicedMemberUploadsAsync_WithSpecifiedAuthority_OOnlyReturnsInvoicedMemberUploadsForTheSpecifiedAuthority()
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
                databaseAuthority1.AnnualChargeAmount = 0;
                wrapper.Model.CompetentAuthorities.Add(databaseAuthority1);

                InvoiceRun invoiceRunForAuthority1 = new InvoiceRun();
                invoiceRunForAuthority1.Id = new Guid("CE7A2617-AE16-403E-A7BF-BF01AD223872");
                invoiceRunForAuthority1.CompetentAuthority = databaseAuthority1;
                invoiceRunForAuthority1.IssuedByUserId = user.Id;
                invoiceRunForAuthority1.IssuedDate = new DateTime(2015, 1, 1);
                wrapper.Model.InvoiceRuns.Add(invoiceRunForAuthority1);

                var scheme = helper.CreateScheme();
                scheme.CompetentAuthorityId = databaseAuthority1.Id;
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme, invoiceRunForAuthority1);

                wrapper.Model.SaveChanges();

                UKCompetentAuthority domainAuthority1 = wrapper.WeeeContext.UKCompetentAuthorities.Find(databaseAuthority1.Id);

                CommonDataAccess dataAccess = new CommonDataAccess(wrapper.WeeeContext);

                // Act
                IReadOnlyList<Domain.Scheme.MemberUpload> results = await dataAccess.FetchInvoicedMemberUploadsAsync(domainAuthority1);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                Assert.Equal(memberUpload.Id, results[0].Id);
            }
        }
    }
}
