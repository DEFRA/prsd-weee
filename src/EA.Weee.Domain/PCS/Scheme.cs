namespace EA.Weee.Domain.PCS
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        public Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public string ApprovalNumber { get; private set; }

        private List<Producer> Producers { get; set; }

        public List<Producer> GetProducersList(int complianceYear)
        {
            var producers = (from producer in Producers
                             where producer.MemberUpload.IsSubmitted && producer.MemberUpload.ComplianceYear == complianceYear
                             group producer by producer.RegistrationNumber into groups
                             select groups.OrderByDescending(p => p.LastSubmittedDate).First()).ToList();

            return producers;
        }

        public string GetProducerCSV(int complianceYear)
        {
            var producers = GetProducersList(complianceYear);

            string[] csvColumnHeaders =
            {
                "Producer Name", "PRN", "Companies house number", "Charge band", "Date registered",
                "Authorised representative", "Overseas producer"
            };

            StringBuilder sb = new StringBuilder();

            for (var i = 0; i <= csvColumnHeaders.Length - 1; i++)
            {
                sb.Append(csvColumnHeaders[i]);

                if (i < csvColumnHeaders.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            for (var i = 0; i <= producers.Count - 1; i++)
            {
                var producer = producers[i];

                var producerName = producer.TradingName;
                var prn = string.IsNullOrEmpty(producer.RegistrationNumber)
                    ? "WEE/********"
                    : producer.RegistrationNumber;
                var companiesHouseNumber = string.Empty;
                if (producer.ProducerBusiness != null)
                {
                    if (producer.ProducerBusiness.CompanyDetails != null)
                    {
                        companiesHouseNumber = producer.ProducerBusiness.CompanyDetails.RegistrationNumber;
                    }
                }
                var chargeBand = "***";
                var dateRegistered = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                var authorisedRepresentative = producer.AuthorisedRepresentative == null ? "No" : "Yes";
                var overseasProducer = producer.AuthorisedRepresentative == null
                    ? string.Empty
                    : producer.AuthorisedRepresentative.OverseasProducerName;

                sb.Append(ReplaceSpecialCharacters(producerName));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(prn));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(companiesHouseNumber));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(chargeBand));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(dateRegistered));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(authorisedRepresentative));
                sb.Append(",");

                sb.Append(ReplaceSpecialCharacters(overseasProducer));

                sb.AppendLine();
            }


            return sb.ToString();
        }

        private string ReplaceSpecialCharacters(string value)
        {
            if (value.Contains(","))
            {
                value = string.Concat("\"", value, "\"");
            }

            if (value.Contains("\r"))
            {
                value = value.Replace("\r", " ");
            }
            if (value.Contains("\n"))
            {
                value = value.Replace("\n", " ");
            }
            return value;
        }
    }
}
