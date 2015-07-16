namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
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
            this.MemberUpload = memberUpload;
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