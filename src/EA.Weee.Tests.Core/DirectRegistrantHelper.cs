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

        public static Tuple<Domain.Organisation.Organisation, DirectRegistrant, Domain.Producer.RegisteredProducer> CreateOrganisationWithRegisteredProducer(DatabaseWrapper wrapper, string companyName, string prn, int complianceYear)
        {
            var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany(companyName, "123456789");
            var address = new Domain.Organisation.Address("1", "street", "Woking", "Hampshire", "GU21 5EE", wrapper.WeeeContext.Countries.First(), "12345678", "test@co.uk");
            organisation.AddOrUpdateAddress(Domain.AddressType.RegisteredOrPPBAddress, address);

            var directRegistrant = DirectRegistrant.CreateDirectRegistrant(organisation, null, null, null, null, null);
            var registeredProducer = new Domain.Producer.RegisteredProducer(prn, complianceYear);

            return Tuple.Create(organisation, directRegistrant, registeredProducer);
        }

        public static async Task<DirectProducerSubmission> CreateSubmission(
            DatabaseWrapper wrapper,
            DirectRegistrant directRegistrant,
            Domain.Producer.RegisteredProducer registeredProducer,
            int complianceYear,
            IEnumerable<EeeOutputAmountData> amounts,
            DirectProducerSubmissionStatus status)
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
            wrapper.WeeeContext.DirectProducerSubmissions.Add(submission);
            await wrapper.WeeeContext.SaveChangesAsync();

            submission.SetCurrentSubmission(history);
            submission.DirectProducerSubmissionStatus = status;
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
