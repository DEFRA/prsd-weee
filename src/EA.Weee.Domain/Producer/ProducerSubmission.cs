namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Lookup;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

    public class ProducerSubmission : Entity, IEquatable<ProducerSubmission>
    {
        public ProducerSubmission(
            RegisteredProducer registeredProducer,
            MemberUpload memberUpload,
            ProducerBusiness producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime updatedDate,
            decimal? annualTurnover,
            bool vatRegistered,
            DateTime? ceaseToExist,
            string tradingName,
            EEEPlacedOnMarketBandType eeePlacedOnMarketBandType,
            SellingTechniqueType sellingTechniqueType,
            ObligationType obligationType,
            AnnualTurnOverBandType annualTurnOverBandType,
            List<BrandName> brandnames,
            List<SICCode> codes,
            ChargeBandAmount chargeBandAmount,
            decimal chargeThisUpdate)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);
            Guard.ArgumentNotNull(() => memberUpload, memberUpload);
            Guard.ArgumentNotNull(() => producerBusiness, producerBusiness);
            Guard.ArgumentNotNull(() => chargeBandAmount, chargeBandAmount);

            if (registeredProducer.Scheme.Id != memberUpload.Scheme.Id)
            {
                string errorMessage = "A producer submission can only be associated "
                    + "with a registered producer in the same scheme.";
                throw new InvalidOperationException(errorMessage);
            }

            if (registeredProducer.ComplianceYear != memberUpload.ComplianceYear)
            {
                string errorMessage = "A producer submission can only be associated "
                    + "with a registered producer in the same compliance year.";
                throw new InvalidOperationException(errorMessage);
            }

            RegisteredProducer = registeredProducer;
            ProducerBusiness = producerBusiness;
            AuthorisedRepresentative = authorisedRepresentative;
            UpdatedDate = updatedDate;
            AnnualTurnover = annualTurnover;
            VATRegistered = vatRegistered;
            CeaseToExist = ceaseToExist;
            TradingName = tradingName;
            EEEPlacedOnMarketBandType = eeePlacedOnMarketBandType.Value;
            SellingTechniqueType = sellingTechniqueType.Value;
            ObligationType = obligationType;
            AnnualTurnOverBandType = annualTurnOverBandType.Value;
            BrandNames = brandnames;
            SICCodes = codes;
            MemberUpload = memberUpload;
            ChargeBandAmount = chargeBandAmount;
            ChargeThisUpdate = chargeThisUpdate;
        }

        protected ProducerSubmission()
        {
        }

        public virtual RegisteredProducer RegisteredProducer { get; private set; }

        public virtual MemberUpload MemberUpload { get; private set; }

        public virtual Guid? AuthorisedRepresentativeId { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public virtual Guid ProducerBusinessId { get; private set; }

        public virtual ProducerBusiness ProducerBusiness { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public string TradingName { get; private set; }

        public bool VATRegistered { get; private set; }
    
        public decimal? AnnualTurnover { get; private set; }

        public DateTime? CeaseToExist { get; private set; }

        public virtual ICollection<BrandName> BrandNames { get; private set; }

        public virtual ICollection<SICCode> SICCodes { get; private set; }

        public int AnnualTurnOverBandType { get; private set; }

        public int EEEPlacedOnMarketBandType { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public int SellingTechniqueType { get; private set; }

        public virtual ChargeBandAmount ChargeBandAmount { get; private set; }

        public decimal ChargeThisUpdate { get; private set; }

        private string organisationName;
        public virtual string OrganisationName
        {
            get
            {
                if (organisationName != null)
                {
                    return organisationName;
                }
                if (ProducerBusiness != null)
                {
                    if (ProducerBusiness.CompanyDetails != null && ProducerBusiness.CompanyDetails.Name != null)
                    {
                        organisationName = ProducerBusiness.CompanyDetails.Name;
                    }
                    else if (ProducerBusiness.Partnership != null && ProducerBusiness.Partnership.Name != null)
                    {
                        organisationName = ProducerBusiness.Partnership.Name;
                    }
                    return organisationName;
                }
                return null;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool Equals(ProducerSubmission other)
        {
            if (other == null)
            {
                return false;
            }

            return TradingName == other.TradingName &&
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
            return Equals(obj as ProducerSubmission);
        }

        /// <summary>
        /// This column should only be used by Entity Framework. it provides a mapping between the
        /// <see cref="ObligationType"/> enum and the NVARCHAR(4) stored in the database.
        /// </summary>
        public string DatabaseObligationType
        {
            get { return ObligationType.ToString(); }
            set { ObligationType = (ObligationType)Enum.Parse(typeof(ObligationType), value); }
        }
    }
}