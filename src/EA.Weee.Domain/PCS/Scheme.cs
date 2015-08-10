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

        public virtual string ApprovalNumber { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public void SetProducers(List<Producer> producers)
        {
            Producers = producers;
        }

        public List<Producer> GetProducersList(int complianceYear)
        {
            return Producers
                .Where(p => p.IsCurrentForComplianceYear)
                .Where(p => p.MemberUpload.ComplianceYear == complianceYear)
                .ToList();
        }

        public string GetProducerCSV(int complianceYear)
        {
            List<Producer> producers = GetProducersList(complianceYear);
            producers = producers.OrderBy(p => p.OrganisationName).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append(Producer.GetCSVColumnHeaders());
            sb.AppendLine();
            foreach (Producer producer in producers)
            {
                sb.Append(producer.ToCsvString());
            }

            return sb.ToString();
        }
    }
}