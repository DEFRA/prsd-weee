namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Organisation;
    using Producer;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Scheme : Entity
    {
        public Scheme(Guid organisationId)
        {
            OrganisationId = organisationId;
            SchemeStatus = SchemeStatus.Pending;
            ApprovalNumber = string.Empty;
            Producers = new List<Producer>();
        }

        protected Scheme()
        {
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual SchemeStatus SchemeStatus { get; private set; }

        public virtual string ApprovalNumber { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public string SchemeName { get; private set; }

        public string IbisCustomerReference { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public Guid CompetentAuthorityId { get; private set; }

        public void UpdateScheme(string schemeName, string ibisCustomerReference, ObligationType obligationType, Guid competentAuthorityId)
        {
            Guard.ArgumentNotNullOrEmpty(() => schemeName, schemeName);
            Guard.ArgumentNotNull(() => obligationType, obligationType);

            SchemeName = schemeName;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthorityId = competentAuthorityId;
        }

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