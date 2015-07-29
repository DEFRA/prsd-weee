namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using PCS;
    using Prsd.Core.Domain;

    public class Producer : Entity
    {
        public Producer(Guid schemeId,
            MemberUpload memberUpload,
            ProducerBusiness producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime lastSubmittedDate,
            decimal annualTurnover,
            bool vatRegistered,
            string registrationNumber,
            DateTime? ceaseToExist,
            string tradingName,
            EEEPlacedOnMarketBandType eeePlacedOnMarketBandType,
            SellingTechniqueType sellingTechniqueType,
            ObligationType obligationType,
            AnnualTurnOverBandType annualTurnOverBandType,
            List<BrandName> brandnames,
            List<SICCode> codes,
            ChargeBandType chargeBandType)
        {
            ProducerBusiness = producerBusiness;
            AuthorisedRepresentative = authorisedRepresentative;

            LastSubmitted = lastSubmittedDate;

            AnnualTurnover = annualTurnover;
            VATRegistered = vatRegistered;
            RegistrationNumber = registrationNumber;
            CeaseToExist = ceaseToExist;
            TradingName = tradingName;

            EEEPlacedOnMarketBandType = eeePlacedOnMarketBandType.Value;
            SellingTechniqueType = sellingTechniqueType.Value;
            ObligationType = obligationType.Value;
            AnnualTurnOverBandType = annualTurnOverBandType.Value;

            BrandNames = brandnames;
            SICCodes = codes;
            SchemeId = schemeId;
            MemberUpload = memberUpload;

            ChargeBandType = chargeBandType.Value;
        }

        protected Producer()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var producerObj = obj as Producer;

            if (producerObj == null)
            {
                return false;
            }
            var compareAuthorisedRepresentative = false;
            if (AuthorisedRepresentative == null && producerObj.AuthorisedRepresentative == null)
            {
                compareAuthorisedRepresentative = true;
            }
            else
            {
                if (AuthorisedRepresentative != null && producerObj.AuthorisedRepresentative != null)
                {
                    compareAuthorisedRepresentative =
                        AuthorisedRepresentative.Equals(producerObj.AuthorisedRepresentative);
                }
            }

            var compareProducerBusiness = false;
            if (ProducerBusiness == null && producerObj.ProducerBusiness == null)
            {
                compareProducerBusiness = true;
            }
            else
            {
                if (ProducerBusiness != null && producerObj.ProducerBusiness != null)
                {
                    compareProducerBusiness =
                        ProducerBusiness.Equals(producerObj.ProducerBusiness);
                }
            }

            var compareBrandName = false;
            if (BrandNames.Count == producerObj.BrandNames.Count)
            {
                BrandNames.Sort();
                producerObj.BrandNames.Sort();
                compareBrandName = BrandNames.SequenceEqual(producerObj.BrandNames);
            }

            var compareSICCodes = false;
            if (SICCodes.Count == producerObj.SICCodes.Count)
            {
                SICCodes.Sort();
                producerObj.SICCodes.Sort();
                compareSICCodes = SICCodes.SequenceEqual(producerObj.SICCodes);
            }

            return RegistrationNumber.Equals(producerObj.RegistrationNumber)
                   && TradingName.Equals(producerObj.TradingName)
                   && VATRegistered.Equals(producerObj.VATRegistered)
                   && AnnualTurnover.Equals(producerObj.AnnualTurnover)
                   && ObligationType.Equals(producerObj.ObligationType)
                   && AnnualTurnOverBandType.Equals(producerObj.AnnualTurnOverBandType)
                   && SellingTechniqueType.Equals(producerObj.SellingTechniqueType)
                   && EEEPlacedOnMarketBandType.Equals(producerObj.EEEPlacedOnMarketBandType)
                   &&
                   compareBrandName && compareSICCodes && compareAuthorisedRepresentative && compareProducerBusiness;
        }

        public virtual Guid SchemeId { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual Guid MemberUploadId { get; private set; }

        public virtual MemberUpload MemberUpload { get; private set; }

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public virtual Guid ProducerBusinessId { get; private set; }

        public virtual ProducerBusiness ProducerBusiness { get; private set; }

        public DateTime LastSubmitted { get; private set; }

        public string RegistrationNumber { get; private set; }

        public string TradingName { get; private set; }

        public bool VATRegistered { get; private set; }

        public decimal AnnualTurnover { get; private set; }

        public DateTime? CeaseToExist { get; private set; }

        public virtual List<BrandName> BrandNames { get; private set; }

        public virtual List<SICCode> SICCodes { get; private set; }

        public int AnnualTurnOverBandType { get; private set; }

        public int EEEPlacedOnMarketBandType { get; private set; }

        public int ObligationType { get; private set; }

        public int SellingTechniqueType { get; private set; }

        public int ChargeBandType { get; private set; }

        public void SetScheme(Scheme scheme)
        {
            Scheme = scheme;
        }

        private DateTime GetProducerRegistrationDate(string registrationNumber, int complianceYear)
        {
            return (from item in Scheme.Producers
                    where item.MemberUpload.IsSubmitted && item.MemberUpload.ComplianceYear == complianceYear && item.RegistrationNumber == registrationNumber
                    select item.LastSubmitted).ToList().OrderBy(ls => ls).First();
        }

        public static string GetCSVColumnHeaders()
        {
            string[] csvColumnHeaders =
            {
                "Organisation Name", "Trading Name", "PRN", "Companies house number", "Charge band", "Date & Time (GMT) Registered",
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

            return sb.ToString();
        }

        public string ToCsvString(Producer producer)
        {
            StringBuilder sb = new StringBuilder();

            var orgName = string.Empty;
            if (producer.ProducerBusiness.CompanyDetails != null &&
                producer.ProducerBusiness.CompanyDetails.Name != null)
            {
                orgName = producer.ProducerBusiness.CompanyDetails.Name;
            }
            else if (producer.ProducerBusiness.Partnership != null &&
                     producer.ProducerBusiness.Partnership.Name != null)
            {
                orgName = producer.ProducerBusiness.Partnership.Name;
            }

            var tradingName = producer.TradingName;
            var prn = string.IsNullOrEmpty(producer.RegistrationNumber)
                ? "WEE/********"
                : producer.RegistrationNumber;
            var companiesHouseNumber = string.Empty;
            if (producer.ProducerBusiness != null && producer.ProducerBusiness.CompanyDetails != null)
            {
                companiesHouseNumber = producer.ProducerBusiness.CompanyDetails.CompanyNumber;
            }
            var chargeBand = "***";
            var dateRegistered = GetProducerRegistrationDate(producer.RegistrationNumber, producer.MemberUpload.ComplianceYear).ToString(CultureInfo.InvariantCulture);
            var authorisedRepresentative = producer.AuthorisedRepresentative == null ? "No" : "Yes";
            var overseasProducer = producer.AuthorisedRepresentative == null
                ? string.Empty
                : producer.AuthorisedRepresentative.OverseasProducerName;

            sb.Append(ReplaceSpecialCharacters(orgName));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(tradingName));
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