namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Domain;
    using Prsd.Core.Domain;
    using Scheme;

    public class Producer : Entity, IEquatable<Producer>
    {
        public Producer(Guid schemeId,
            MemberUpload memberUpload,
            ProducerBusiness producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime updatedDate,
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
            bool isCurrentForComplianceYear,
            ChargeBandType chargeBandType,
            decimal chargeThisUpdate)
        {
            ProducerBusiness = producerBusiness;
            AuthorisedRepresentative = authorisedRepresentative;

            UpdatedDate = updatedDate;

            AnnualTurnover = annualTurnover;
            VATRegistered = vatRegistered;
            RegistrationNumber = registrationNumber;
            CeaseToExist = ceaseToExist;
            TradingName = tradingName;

            EEEPlacedOnMarketBandType = eeePlacedOnMarketBandType.Value;
            SellingTechniqueType = sellingTechniqueType.Value;
            ObligationType = (int)obligationType;
            AnnualTurnOverBandType = annualTurnOverBandType.Value;

            BrandNames = brandnames;
            SICCodes = codes;
            SchemeId = schemeId;
            MemberUpload = memberUpload;

            IsCurrentForComplianceYear = isCurrentForComplianceYear;

            ChargeBandType = chargeBandType.Value;
            ChargeThisUpdate = chargeThisUpdate;
        }

        protected Producer()
        {
        }

        public virtual Guid SchemeId { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual Guid MemberUploadId { get; private set; }

        public virtual MemberUpload MemberUpload { get; private set; }

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public virtual Guid ProducerBusinessId { get; private set; }

        public virtual ProducerBusiness ProducerBusiness { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public virtual string RegistrationNumber { get; private set; }

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

        public decimal ChargeThisUpdate { get; private set; }

        private string OrgName { get; set; }

        public string OrganisationName
        {
            get
            {
                if (OrgName != null)
                {
                    return OrgName;
                }
                if (ProducerBusiness != null)
                {
                    if (ProducerBusiness.CompanyDetails != null && ProducerBusiness.CompanyDetails.Name != null)
                    {
                        OrgName = ProducerBusiness.CompanyDetails.Name;
                    }
                    else if (ProducerBusiness.Partnership != null && ProducerBusiness.Partnership.Name != null)
                    {
                        OrgName = ProducerBusiness.Partnership.Name;
                    }
                    return OrgName;
                }
                return null;
            }
        }

        /// <summary>
        /// Indicates whether this data is current. I.e. no data has been submitted
        /// for a producer with the same registration number, scheme and compliance year
        /// since this data was submitted.
        /// 
        /// If results are filtered by this property, the results are guarenteed to
        /// be unique accross registration number, scheme and compliance year.
        /// 
        /// A filtered index in the database has been provided to ensure that such queries
        /// are efficient at including only current producers.
        /// </summary>
        public virtual bool IsCurrentForComplianceYear { get; private set; }

        public void SetScheme(Scheme scheme)
        {
            Scheme = scheme;
        }

        private DateTime GetProducerRegistrationDate(string registrationNumber, int complianceYear)
        {
            return (from item in Scheme.Producers
                    where item.MemberUpload.IsSubmitted && item.MemberUpload.ComplianceYear == complianceYear && item.RegistrationNumber == registrationNumber
                    select item.UpdatedDate).ToList().OrderBy(ls => ls).First();
        }

        public static string GetCSVColumnHeaders()
        {
            string[] csvColumnHeaders =
            {
                "Organisation Name", 
                "Trading Name", 
                "PRN", 
                "Companies house number", 
                "Charge band", 
                "Date & Time (GMT) Registered", 
                "Date & Time (GMT) Last Updated",
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

        public string ToCsvString()
        {
            StringBuilder sb = new StringBuilder();

            var companiesHouseNumber = ProducerBusiness != null && ProducerBusiness.CompanyDetails != null
                ? ProducerBusiness.CompanyDetails.CompanyNumber
                : string.Empty;

            var chargeBand = Enumeration.FromValue<ChargeBandType>(ChargeBandType).DisplayName;

            var dateRegistered = string.Format("{0:dd/MM/yyyy HH:mm:ss}", GetProducerRegistrationDate(RegistrationNumber, MemberUpload.ComplianceYear.Value));

            var dateAmended = string.Format("{0:dd/MM/yyyy HH:mm:ss}", UpdatedDate);
            if (dateRegistered == dateAmended)
            {
                dateAmended = string.Empty;
            }

            var authorisedRepresentative = AuthorisedRepresentative == null ? "No" : "Yes";

            var overseasProducer = AuthorisedRepresentative == null
                ? string.Empty
                : AuthorisedRepresentative.OverseasProducerName;

            sb.Append(ReplaceSpecialCharacters(OrganisationName));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(TradingName));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(RegistrationNumber));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(companiesHouseNumber));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(chargeBand));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(dateRegistered));
            sb.Append(",");

            sb.Append(ReplaceSpecialCharacters(dateAmended));
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Producer other)
        {
            if (other == null)
            {
                return false;
            }

            return RegistrationNumber == other.RegistrationNumber &&
                   TradingName == other.TradingName &&
                   VATRegistered == other.VATRegistered &&
                   AnnualTurnover == other.AnnualTurnover &&
                   ObligationType == other.ObligationType &&
                   AnnualTurnOverBandType == other.AnnualTurnOverBandType &&
                   SellingTechniqueType == other.SellingTechniqueType &&
                   EEEPlacedOnMarketBandType == other.EEEPlacedOnMarketBandType &&
                   object.Equals(AuthorisedRepresentative, other.AuthorisedRepresentative) &&
                   object.Equals(ProducerBusiness, other.ProducerBusiness) &&
                   BrandNames.ElementsEqual(other.BrandNames) &&
                   SICCodes.ElementsEqual(other.SICCodes);
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Producer);
        }
    }
}