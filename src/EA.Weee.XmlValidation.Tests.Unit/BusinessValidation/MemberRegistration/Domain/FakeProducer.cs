namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Domain
{
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Domain;
    using Weee.Domain.Lookup;
    using Weee.Domain.Producer;
    using Weee.Domain.Scheme;

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
            isCurrentForComplianceYear,
            A.Dummy<ChargeBandAmount>(),
            (decimal)30.0)
        {
            this.schemeOrganisationId = schemeOrganisationId;
        }

        public static FakeProducer Create(ObligationType obligationType, string prn, Guid? schemeOrganisationId = null,
            params string[] brandNames)
        {
            return new FakeProducer(schemeOrganisationId ?? Guid.NewGuid(),
                Guid.NewGuid(),
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name"),
                new ProducerBusiness(),
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                decimal.Zero,
                true,
                prn,
                null,
                "trading name",
                Weee.Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Weee.Domain.SellingTechniqueType.Both,
                obligationType,
                Weee.Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                true);
        }

        public override Scheme Scheme
        {
            get { return new Scheme(schemeOrganisationId); }
        }

        public static FakeProducer Create(ObligationType obligationType, string prn, bool iscurrentcomplainceYear, 
          Guid? schemeOrganisationId = null, int? complianceYear = null, ProducerBusiness producerBusiness = null, params string[] brandNames)
        {
            return new FakeProducer(schemeOrganisationId ?? Guid.NewGuid(),
                Guid.NewGuid(),
                new MemberUpload(Guid.NewGuid(), "<xml>SomeData</xml>", new List<MemberUploadError>(), 0, complianceYear ?? 2016, Guid.NewGuid(), "File name"),
                producerBusiness ?? new ProducerBusiness(new Company("A company name", "ABC12345", null), new Partnership("Partnership Name", null, new List<Partner>())),
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                decimal.Zero,
                true,
                prn,
                null,
                "trading name",
                Weee.Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Weee.Domain.SellingTechniqueType.Both,
                obligationType,
                Weee.Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                iscurrentcomplainceYear);
        }
    }
}
