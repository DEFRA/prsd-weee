namespace EA.Weee.Domain.Producer
{
    using Domain;
    using Lookup;
    using Prsd.Core.Domain;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Producer : Entity, IEquatable<Producer>
    {
        public Producer(Guid schemeId,
            MemberUpload memberUpload,
            ProducerBusiness producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime updatedDate,
            decimal? annualTurnover,
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
            ChargeBandAmount chargeBandAmount,
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

            ChargeBandAmount = chargeBandAmount;
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
    
        public decimal? AnnualTurnover { get; private set; }

        public DateTime? CeaseToExist { get; private set; }

        public virtual List<BrandName> BrandNames { get; private set; }

        public virtual List<SICCode> SICCodes { get; private set; }

        public int AnnualTurnOverBandType { get; private set; }

        public int EEEPlacedOnMarketBandType { get; private set; }

        public int ObligationType { get; private set; }

        public int SellingTechniqueType { get; private set; }

        public virtual ChargeBandAmount ChargeBandAmount { get; private set; }

        public decimal ChargeThisUpdate { get; private set; }

        private string OrgName { get; set; }

        public virtual string OrganisationName
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
        /// 
        /// This property is kept up to date by the MemberUploadSubmittedEventHandler
        /// and should not be changed anywhere else.
        /// </summary>
        public virtual bool IsCurrentForComplianceYear { get; set; }

        public void SetScheme(Scheme scheme)
        {
            Scheme = scheme;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool Equals(Producer other)
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