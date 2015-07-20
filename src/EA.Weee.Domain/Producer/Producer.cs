namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PCS;
    using Prsd.Core.Domain;

    public class Producer : Entity
    {
        public Producer(Guid schemeId, MemberUpload memberUpload,
            ProducerBusiness producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime lastSubmittedDate, decimal annualTurnover,
            bool vatRegistered, string registrationNumber,
            DateTime? ceaseToExist, string tradingName,
            EEEPlacedOnMarketBandType eeePlacedOnMarketBandType,
            SellingTechniqueType sellingTechniqueType,
            ObligationType obligationType,
            AnnualTurnOverBandType annualTurnOverBandType,
            List<BrandName> brandnames, List<SICCode> codes)
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
            var compareAuthorisedRepresentative = true;
            if (AuthorisedRepresentative != null)
            {
                compareAuthorisedRepresentative =
                    AuthorisedRepresentative.Equals(producerObj.AuthorisedRepresentative);
            }

            var compareProducerBusiness = false;
            if (ProducerBusiness != null)
            {
                compareProducerBusiness =
                    ProducerBusiness.Equals(producerObj.ProducerBusiness);
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
    }
}