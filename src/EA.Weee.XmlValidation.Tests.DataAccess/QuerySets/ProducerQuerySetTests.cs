namespace EA.Weee.XmlValidation.Tests.DataAccess.BusinessValidation.Rules.QuerySets
{
    using DataAccess;
    using Domain;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Weee.DataAccess;
    using Weee.Domain.Producer;
    using Weee.Tests.Core;
    using XmlValidation.BusinessValidation.QuerySets;
    using XmlValidation.BusinessValidation.QuerySets.Queries.Producer;
    using Xunit;
    using Assert = Xunit.Assert;

    public class ProducerQuerySetTests
    {
        [Fact]
        public void GetLatestProducerForComplianceYearAndScheme_AllParametersMatch_ReturnsProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "AAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerForComplianceYearAndScheme("AAAAAAA", "2016", scheme.OrganisationId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(producer.Id, result.Id);
            }
        }

        [Fact]
        public void GetLatestProducerForComplianceYearAndScheme_ComplianceYearDoesNotMatch_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "AAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerForComplianceYearAndScheme("AAAAAAA", "2017", scheme.OrganisationId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void GetLatestProducerForComplianceYearAndScheme_PrnDoesNotMatch_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "AAAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerForComplianceYearAndScheme("XXXXXXXX", "2016", scheme.OrganisationId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_PrnDoesNotMatch_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsCompany(memberUpload, "AAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerFromPreviousComplianceYears("XXXXXXX");

                // Assert
                Assert.Null(result);
            } 
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesInConsecutiveYears_ReturnsLatestProducerByComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerFromPreviousComplianceYears("AAAAAAA");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(producer2.Id, result.Id);
            }
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_ReturnsLatestProducerByUdatedDate()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1);

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAAAA");
                producer2.UpdatedDate = new DateTime(2015, 1, 2);

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerFromPreviousComplianceYears("AAAAAAA");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(producer2.Id, result.Id);
            }
        }

        [Fact]
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_AndOneIn2014_ReturnsLatestProducerByUpdatedDateIn2015()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1);

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAAAA");
                producer2.UpdatedDate = new DateTime(2015, 1, 2);

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2014;
                memberUpload3.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer3 = helper.CreateProducerAsCompany(memberUpload3, "AAAAAAA");
                producer3.UpdatedDate = new DateTime(2014, 1, 1);

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetLatestProducerFromPreviousComplianceYears("AAAAAAA");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(producer2.Id, result.Id);
            }
        }

        [Fact]
        public void GetAllRegistrationNumbers_ReturnsDistinctRegistrationNumbers()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = false;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAAAA");

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                List<string> results = querySet.GetAllRegistrationNumbers();

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count(r => r == "AAAAAAA"));
            }
        }

        [Fact]
        public void GetProducerForOtherSchemeAndObligationType_ForAnotherSchemeSameObligationType_ReturnsAnotherSchemeProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme1 = helper.CreateScheme();
                
                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme2);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");
                producer1.ObligationType = "B2C";

                database.Model.SaveChanges();

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                var result = querySet.GetProducerForOtherSchemeAndObligationType("AAAAAAA", "2015", scheme1.OrganisationId, ObligationType.B2C);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(producer1.Id, result.Id);
            }
        }

        [Fact]
        public void GetLatestCompanyProducers_ReturnsCompaniesOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission companyProducer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");

                Weee.Tests.Core.Model.ProducerSubmission partnershipProducer = helper.CreateProducerAsPartnership(memberUpload1, "PPP1");

                Weee.Tests.Core.Model.ProducerSubmission soleTraderProducer = helper.CreateProducerAsSoleTrader(memberUpload1, "SSS1");

                Weee.Tests.Core.Model.ProducerSubmission companyProducer2 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");

                database.Model.SaveChanges();

                // Act
                var result = ProducerQuerySet(database.WeeeContext).GetLatestCompanyProducers();

                // Assert
                Assert.Contains(result, p => p.Id == companyProducer1.Id);
                Assert.Contains(result, p => p.Id == companyProducer2.Id);
                Assert.DoesNotContain(result, p => p.Id == soleTraderProducer.Id);
                Assert.DoesNotContain(result, p => p.Id == partnershipProducer.Id);
            }
        }

        [Fact]
        public void GetLatestCompanyProducers_ReturnsCurrentForComplianceYearCompaniesOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission companyProducer1 = helper.CreateProducerAsCompany(memberUpload1, "AA");

                Weee.Tests.Core.Model.ProducerSubmission companyProducer2 = helper.CreateProducerAsCompany(memberUpload1, "BB");

                Weee.Tests.Core.Model.ProducerSubmission companyProducer3 = helper.CreateProducerAsCompany(memberUpload1, "CC");

                database.Model.SaveChanges();

                // Act
                var result = ProducerQuerySet(database.WeeeContext).GetLatestCompanyProducers();

                // Assert
                Assert.Contains(result, p => p.Id == companyProducer1.Id);
                Assert.Contains(result, p => p.Id == companyProducer3.Id);
                Assert.DoesNotContain(result, p => p.Id == companyProducer2.Id);
            }
        }

        private ProducerQuerySet ProducerQuerySet(WeeeContext context)
        {
            return new ProducerQuerySet(new CurrentProducersByRegistrationNumber(context),
                new ExistingProducerNames(context),
                new ExistingProducerRegistrationNumbers(context),
                new CurrentCompanyProducers(context));
        }
    }
}
