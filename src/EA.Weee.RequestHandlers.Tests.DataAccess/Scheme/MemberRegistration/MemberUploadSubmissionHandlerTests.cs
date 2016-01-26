namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.User;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Shared.DomainUser;
    using Requests.Scheme.MemberRegistration;
    using Security;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class MemberUploadSubmissionHandlerTests
    {
        [Fact]
        public async Task MemberUploadSubmissionHandler_SubmitMemberUpload_NewRegisteredProducer_ContainsCorrectValueFor_CurrentSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                string registrationNumber = "AAAA";
                int complianceYear = 2016;

                var registeredProducer = helper.GetOrCreateRegisteredProducer(scheme, complianceYear, registrationNumber);

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = complianceYear;
                memberUpload.IsSubmitted = false;

                var producerSubmission = helper.CreateProducerAsCompany(memberUpload, registrationNumber);

                // At least one user is required in the database.
                helper.GetOrCreateUser("A user");

                database.Model.SaveChanges();

                User user = await database.WeeeContext.Users.FirstAsync();
                IDomainUserContext domainUserContext = A.Fake<IDomainUserContext>();
                A.CallTo(() => domainUserContext.GetCurrentUserAsync()).Returns(user);

                var handler = new MemberUploadSubmissionHandler(A.Dummy<IWeeeAuthorization>(), database.WeeeContext, domainUserContext);
                await handler.HandleAsync(new MemberUploadSubmission(scheme.OrganisationId, memberUpload.Id));

                var registeredProducerDb = FindRegisteredProducer(database, registeredProducer.Id);

                Assert.Equal(registeredProducerDb.CurrentSubmission.Id, producerSubmission.Id);
            }
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_SubmitMemberUpload_ExistingRegisteredProducer_CurrentSubmissionValueIsUpdated()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                string registrationNumber = "AAAA";
                int complianceYear = 2016;

                var registeredProducer = helper.GetOrCreateRegisteredProducer(scheme, complianceYear, registrationNumber);

                var memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = true;

                helper.CreateProducerAsCompany(memberUpload1, registrationNumber);

                var memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = false;

                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, registrationNumber);

                // At least one user is required in the database.
                helper.GetOrCreateUser("A user");

                database.Model.SaveChanges();

                User user = await database.WeeeContext.Users.FirstAsync();
                IDomainUserContext domainUserContext = A.Fake<IDomainUserContext>();
                A.CallTo(() => domainUserContext.GetCurrentUserAsync()).Returns(user);

                var handler = new MemberUploadSubmissionHandler(A.Dummy<IWeeeAuthorization>(), database.WeeeContext, domainUserContext);
                await handler.HandleAsync(new MemberUploadSubmission(scheme.OrganisationId, memberUpload2.Id));

                var registeredProducerDb = FindRegisteredProducer(database, registeredProducer.Id);

                Assert.Equal(registeredProducerDb.CurrentSubmission.Id, producerSubmission2.Id);
            }
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_SubmitMemberUpload_DoesNotUpdateRegisteredProducerRecord_ForDifferentComplianceYear()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                string registrationNumber = "AAAA";

                var memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                var registeredProducer1 = helper.GetOrCreateRegisteredProducer(scheme, 2016, registrationNumber);
                var producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, registrationNumber);

                var memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = false;

                var registeredProducer2 = helper.GetOrCreateRegisteredProducer(scheme, 2017, registrationNumber);
                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, registrationNumber);

                // At least one user is required in the database.
                helper.GetOrCreateUser("A user");

                database.Model.SaveChanges();

                User user = await database.WeeeContext.Users.FirstAsync();
                IDomainUserContext domainUserContext = A.Fake<IDomainUserContext>();
                A.CallTo(() => domainUserContext.GetCurrentUserAsync()).Returns(user);

                var handler = new MemberUploadSubmissionHandler(A.Dummy<IWeeeAuthorization>(), database.WeeeContext, domainUserContext);
                await handler.HandleAsync(new MemberUploadSubmission(scheme.OrganisationId, memberUpload2.Id));

                var registeredProducerDb1 = FindRegisteredProducer(database, registeredProducer1.Id);
                var registeredProducerDb2 = FindRegisteredProducer(database, registeredProducer2.Id);

                Assert.Equal(registeredProducerDb1.CurrentSubmission.Id, producerSubmission1.Id);
                Assert.Equal(registeredProducerDb2.CurrentSubmission.Id, producerSubmission2.Id);
            }
        }

        [Fact]
        public async Task MemberUploadSubmissionHandler_SubmitMemberUpload_DoesNotUpdateRegisteredProducerRecord_ForDifferentScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                string registrationNumber = "AAAA";
                int complianceYear = 2016;

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = true;

                var registeredProducer1 = helper.GetOrCreateRegisteredProducer(scheme1, complianceYear, registrationNumber);
                var producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, registrationNumber);

                var scheme2 = helper.CreateScheme();

                var memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = false;

                var registeredProducer2 = helper.GetOrCreateRegisteredProducer(scheme2, complianceYear, registrationNumber);
                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, registrationNumber);

                // At least one user is required in the database.
                helper.GetOrCreateUser("A user");

                database.Model.SaveChanges();

                User user = await database.WeeeContext.Users.FirstAsync();
                IDomainUserContext domainUserContext = A.Fake<IDomainUserContext>();
                A.CallTo(() => domainUserContext.GetCurrentUserAsync()).Returns(user);

                var handler = new MemberUploadSubmissionHandler(A.Dummy<IWeeeAuthorization>(), database.WeeeContext, domainUserContext);
                await handler.HandleAsync(new MemberUploadSubmission(scheme2.OrganisationId, memberUpload2.Id));

                var registeredProducerDb1 = FindRegisteredProducer(database, registeredProducer1.Id);
                var registeredProducerDb2 = FindRegisteredProducer(database, registeredProducer2.Id);

                Assert.Equal(registeredProducerDb1.CurrentSubmission.Id, producerSubmission1.Id);
                Assert.Equal(registeredProducerDb2.CurrentSubmission.Id, producerSubmission2.Id);
            }
        }

        private Domain.Producer.ProducerSubmission FindProducerSubmission(DatabaseWrapper wrapper, Guid producerSubmissionId)
        {
            return wrapper.WeeeContext.ProducerSubmissions.First(p => p.Id == producerSubmissionId);
        }

        private Domain.Producer.RegisteredProducer FindRegisteredProducer(DatabaseWrapper wrapper, Guid registeredProducerId)
        {
            return wrapper.WeeeContext.RegisteredProducers.First(p => p.Id == registeredProducerId);
        }
    }
}