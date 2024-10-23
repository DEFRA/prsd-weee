﻿namespace EA.Weee.XmlValidation.Tests.DataAccess.QuerySets
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries.Producer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        public void GetLatestProducerFromPreviousComplianceYears_TwoProducerEntriesIn2015_ReturnsLatestProducerByUpdatedDate()
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
                memberUpload1.HasAnnualCharge = false;

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAA");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;
                memberUpload2.HasAnnualCharge = false;

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
        public async Task GetAllRegistrationNumbersWithSmallProducerSubmission_ReturnsDistinctRegistrationNumbers()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(database);

                var complianceYear = 2060;

                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "My company", "WEE/AG48365JN", complianceYear - 1);

                var submission1 = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete);

                ProducerQuerySet querySet = ProducerQuerySet(database.WeeeContext);

                // Act
                List<string> results = querySet.GetAllRegistrationNumbers();

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count(r => r == "WEE/AG48365JN"));
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

                Weee.Tests.Core.Model.ProducerSubmission companyProducer2 = helper.CreateProducerAsCompany(memberUpload1, "AAAAAAB");

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

                Weee.Tests.Core.Model.ProducerSubmission companyProducer3 = helper.CreateProducerAsCompany(memberUpload1, "AA");

                database.Model.SaveChanges();

                // Act
                var result = ProducerQuerySet(database.WeeeContext).GetLatestCompanyProducers();

                // Assert
                Assert.DoesNotContain(result, p => p.Id == companyProducer1.Id);
                Assert.Contains(result, p => p.Id == companyProducer2.Id);
                Assert.Contains(result, p => p.Id == companyProducer3.Id);
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
