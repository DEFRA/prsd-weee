namespace EA.Weee.Domain.PCS
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Organisation;
    using Producer;

    public class Scheme : Entity
    {
        public Scheme(Guid organisationId)
        {
            OrganisationId = organisationId;
            ApprovalNumber = string.Empty;
            Producers = new List<Producer>();
        }

        protected Scheme()
        {
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public string ApprovalNumber { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public void SetProducers(List<Producer> producers)
        {
            Producers = producers;
        }

        public List<Producer> GetProducersList(int complianceYear)
        {
            var producers = (from producer in Producers
                             where producer.MemberUpload.IsSubmitted && producer.MemberUpload.ComplianceYear == complianceYear
                             group producer by producer.RegistrationNumber into groups
                             select groups.OrderByDescending(p => p.LastSubmitted).First()).ToList();

            return producers;
        }

        public string GetProducerCSV(int complianceYear)
        {
            var sb = new StringBuilder();

            var producers = GetProducersList(complianceYear);

            var csvColumnHeaders = Producer.GetCSVColumnHeaders();

            sb.Append(csvColumnHeaders);

            sb.AppendLine();

            for (var i = 0; i <= producers.Count - 1; i++)
            {
                var producer = producers[i];
                sb.Append(producer.ToCsvString(producer));
            }

            return sb.ToString();
        }
    }
}