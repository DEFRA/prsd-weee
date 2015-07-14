namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Collections.Generic;
    using PCS;
    using Prsd.Core.Domain;

    public class Producer : Entity
    {
        public Producer(MemberUpload memberUpload, Business producerBusiness,
            AuthorisedRepresentative authorisedRepresentative,
            DateTime lastSubmittedDate, decimal annualTurnover, bool vatRegistered, string registrationNumber,
            Scheme scheme, DateTime ceaseToExist, string tradingName)
        {
            MemberUpload = memberUpload;
            ProducerBusiness = producerBusiness;
            AuthorisedRepresentative = authorisedRepresentative;
            LastSubmittedDate = lastSubmittedDate;
            AnnualTurnover = annualTurnover;
            VATRegistered = vatRegistered;
            RegistrationNumber = registrationNumber;
            Scheme = scheme;
            CeaseToExist = ceaseToExist;
            TradingName = tradingName;
            BrandNames = new List<BrandName>();
            SICCodes = new List<SICCode>();
        }

         protected Producer()
        {
        }

        public virtual Scheme Scheme { get; private set; }

        public virtual MemberUpload MemberUpload { get; private set; }

        public virtual AuthorisedRepresentative AuthorisedRepresentative { get; private set; }

        public virtual Business ProducerBusiness { get; private set; }

        public DateTime LastSubmittedDate { get; private set; }

        public string RegistrationNumber { get; private set; }

        public string TradingName { get; private set; }

        public bool VATRegistered { get; private set; }

        public decimal AnnualTurnover { get; private set; }

        public DateTime CeaseToExist { get; private set; }

        public virtual List<BrandName> BrandNames { get; private set; }

        public virtual List<SICCode> SICCodes { get; private set; }

        public EEEPlacedOnMarketBandType EEEPlacedOnMarketBandType { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public SellingTechniqueType SellingTechniqueType { get; private set; }
    }
}