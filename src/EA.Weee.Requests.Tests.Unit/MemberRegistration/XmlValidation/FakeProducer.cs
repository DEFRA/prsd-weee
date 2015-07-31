namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;

    public class FakeProducer : Producer
    {
        private readonly Guid schemeOrganisationId;

        private FakeProducer(Guid schemeOrganisationId,
            Guid schemeId, 
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
            List<BrandName> brandNames,
            List<SICCode> codes,
            bool isCurrentForComplianceYear) 
            : base(schemeId, 
            memberUpload, 
            producerBusiness, 
            authorisedRepresentative, 
            lastSubmittedDate, 
            annualTurnover, 
            vatRegistered, 
            registrationNumber, 
            ceaseToExist, 
            tradingName, 
            eeePlacedOnMarketBandType, 
            sellingTechniqueType, 
            obligationType, 
            annualTurnOverBandType, 
            brandNames, 
            codes,
            isCurrentForComplianceYear)
        {
            this.schemeOrganisationId = schemeOrganisationId;
        }

        public static FakeProducer Create(ObligationType obligationType, string prn, Guid? schemeOrganisationId = null,
            params string[] brandNames)
        {
            return new FakeProducer(schemeOrganisationId ?? Guid.NewGuid(),
                Guid.NewGuid(),
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>", new List<MemberUploadError>()),
                new ProducerBusiness(),
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                decimal.Zero,
                true,
                prn,
                null,
                "trading name",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                obligationType,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                true);
        }

        public override Scheme Scheme
        {
            get { return new Scheme(schemeOrganisationId); }
        }

        public static FakeProducer Create(ObligationType obligationType, string prn, bool iscurrentcomplainceYear, 
          Guid? schemeOrganisationId = null,  params string[] brandNames)
        {
            return new FakeProducer(schemeOrganisationId ?? Guid.NewGuid(),
                Guid.NewGuid(),
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>", new List<MemberUploadError>()),
                new ProducerBusiness(),
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                decimal.Zero,
                true,
                prn,
                null,
                "trading name",
                Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Domain.SellingTechniqueType.Both,
                obligationType,
                Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                iscurrentcomplainceYear);
        }
    }
}
