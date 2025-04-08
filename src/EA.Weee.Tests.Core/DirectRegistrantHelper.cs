namespace EA.Weee.Tests.Core
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class DirectRegistrantHelper
    {
        public static Tuple<int, Domain.Country> SetupCommonTestData(DatabaseWrapper wrapper)
        {
            var user = wrapper.Model.AspNetUsers.First().Id;
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(Guid.Parse(user));
            var complianceYear = SystemTime.UtcNow.Year;
            var country = wrapper.WeeeContext.Countries.First();

            return Tuple.Create(complianceYear, country);
        }

        public static Tuple<Domain.Organisation.Organisation, DirectRegistrant, Domain.Producer.RegisteredProducer> CreateOrganisationWithRegisteredProducer(DatabaseWrapper wrapper, string companyName, string prn, int complianceYear, string companyNumber = null, Domain.Producer.AuthorisedRepresentative authorisedRepresentative = null, BrandName brandNames = null, string tradingName = null)
        {
            companyNumber = companyNumber ?? "123456789";
            var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany(companyName, "123456789", tradingName);
            var address = new Domain.Organisation.Address("primary 1", "street", "Woking", "Hampshire", "GU21 5EE", wrapper.WeeeContext.Countries.First(), "12345678", "test@co.uk");
            organisation.AddOrUpdateAddress(Domain.AddressType.RegisteredOrPPBAddress, address);

            var contact = new Domain.Organisation.Contact("first name", "last name", "position");

            var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, brandNames, contact, address, authorisedRepresentative, null, null);

            var registeredProducer = new Domain.Producer.RegisteredProducer(prn, complianceYear);

            return Tuple.Create(organisation, directRegistrant, registeredProducer);
        }

        public static async Task<DirectProducerSubmission> CreateSubmission(
            DatabaseWrapper wrapper,
            DirectRegistrant directRegistrant,
            Domain.Producer.RegisteredProducer registeredProducer,
            int complianceYear,
            IEnumerable<EeeOutputAmountData> amounts,
            DirectProducerSubmissionStatus status,
            int? sellingTechniqueType = null,
            bool paid = false)
        {
            var submission = new DirectProducerSubmission
            {
                ComplianceYear = complianceYear,
                RegisteredProducer = registeredProducer,
                DirectRegistrant = directRegistrant
            };

            var history = new DirectProducerSubmissionHistory(submission);
            
            var returnVersion = new Domain.DataReturns.EeeOutputReturnVersion();

            foreach (var amount in amounts)
            {
                returnVersion.EeeOutputAmounts.Add(new Domain.DataReturns.EeeOutputAmount(amount.ObligationType, amount.Category, amount.Amount, registeredProducer));
            }

            history.EeeOutputReturnVersion = returnVersion;

            if (status == DirectProducerSubmissionStatus.Complete)
            {
                history.SubmittedDate = SystemTime.UtcNow;
            }

            if (sellingTechniqueType.HasValue)
            {
                history.SellingTechniqueType = sellingTechniqueType;
            }
            
            wrapper.WeeeContext.DirectProducerSubmissions.Add(submission);
            await wrapper.WeeeContext.SaveChangesAsync();

            submission.SetCurrentSubmission(history);
            
            submission.DirectProducerSubmissionStatus = status;

            if (paid)
            {
                submission.PaymentFinished = true;
            }

            await wrapper.WeeeContext.SaveChangesAsync();

            return submission;
        }

        public static async Task<DirectProducerSubmission> SubmitSubmission(
            DatabaseWrapper wrapper,
            DirectProducerSubmission submission,
            IEnumerable<EeeOutputAmountData> amounts,
            int? sellingTechniqueType = null)
        {
            var returnVersion = new Domain.DataReturns.EeeOutputReturnVersion();

            foreach (var amount in amounts)
            {
                returnVersion.EeeOutputAmounts.Add(new Domain.DataReturns.EeeOutputAmount(amount.ObligationType, amount.Category, amount.Amount, submission.RegisteredProducer));
            }

            submission.CurrentSubmission.EeeOutputReturnVersion = returnVersion;
            submission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete;
            submission.CurrentSubmission.SubmittedDate = SystemTime.UtcNow;
            
            if (sellingTechniqueType.HasValue)
            {
                submission.CurrentSubmission.SellingTechniqueType = sellingTechniqueType;
            }

            await wrapper.WeeeContext.SaveChangesAsync();

            return submission;
        }

        public static async Task<PaymentSession> CreatePaymentSession(
            DatabaseWrapper wrapper,
            DirectProducerSubmission submission,
            DateTime? updatedDate = null)
        {
            var paymentSession = new PaymentSession(wrapper.WeeeContext.GetCurrentUser(), 10, DateTime.UtcNow,
                submission, submission.DirectRegistrant, "token", "paymentid", "ref");

            if (updatedDate.HasValue)
            {
                paymentSession.UpdatedAt = updatedDate;
            }

            wrapper.WeeeContext.PaymentSessions.Add(paymentSession);

            await wrapper.WeeeContext.SaveChangesAsync();

            return paymentSession;
        }

        public static async Task<DirectProducerSubmission> SetSubmissionAsPaid(
            DatabaseWrapper wrapper,
            DirectProducerSubmission submission,
            DateTime? manualPaymentDate = null)
        {
            submission.PaymentFinished = true;

            if (manualPaymentDate.HasValue)
            {
                submission.ManualPaymentReceivedDate = manualPaymentDate;
            }

            await wrapper.WeeeContext.SaveChangesAsync();

            return submission;
        }

        public static async Task<DirectProducerSubmission> ReturnSubmission(
            DatabaseWrapper wrapper,
            DirectProducerSubmission submission)
        {
            var history = new DirectProducerSubmissionHistory(submission);
            
            wrapper.WeeeContext.DirectProducerSubmissionHistories.Add(history);
            await wrapper.WeeeContext.SaveChangesAsync();

            submission.SetCurrentSubmission(history);

            submission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned;
            await wrapper.WeeeContext.SaveChangesAsync();

            return submission;
        }

        public static async Task<DirectProducerSubmission> UpdateEeeeAmounts(
            DatabaseWrapper wrapper,
            DirectProducerSubmission submission,
            IEnumerable<EeeOutputAmountData> amounts)
        {
            var returnVersion = new Domain.DataReturns.EeeOutputReturnVersion();

            foreach (var amount in amounts)
            {
                returnVersion.EeeOutputAmounts.Add(new Domain.DataReturns.EeeOutputAmount(amount.ObligationType, amount.Category, amount.Amount, submission.RegisteredProducer));
            }

            submission.CurrentSubmission.EeeOutputReturnVersion = returnVersion;

            await wrapper.WeeeContext.SaveChangesAsync();

            return submission;
        }

        public class EeeOutputAmountData
        {
            public WeeeCategory Category { get; set; }
            public decimal Amount { get; set; }
            public Domain.Obligation.ObligationType ObligationType { get; set; }
        }
    }
}
