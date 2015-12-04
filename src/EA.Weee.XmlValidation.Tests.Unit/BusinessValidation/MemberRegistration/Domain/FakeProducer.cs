namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.MemberRegistration.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Weee.Domain;
    using Weee.Domain.Lookup;
    using Weee.Domain.Producer;
    using Weee.Domain.Scheme;

    public class FakeProducer
    {
        public static ProducerSubmission Create(
            ObligationType obligationType,
            string prn,
            Guid? schemeOrganisationId = null,
            int? complianceYear = null,
            ProducerBusiness producerBusiness = null,
            params string[] brandNames)
        {
            Scheme scheme = new Scheme(
                schemeOrganisationId ?? Guid.Empty);

            RegisteredProducer registeredProducer = new RegisteredProducer(
                prn,
                complianceYear ?? 2016,
                scheme);

            MemberUpload memberUpload = new MemberUpload(
                Guid.NewGuid(),
                "<xml>SomeData</xml>",
                new List<MemberUploadError>(),
                0,
                complianceYear ?? 2016,
                scheme,
                "File name");

            return new ProducerSubmission(
                registeredProducer,
                memberUpload,
                producerBusiness ?? new ProducerBusiness(),
                new AuthorisedRepresentative("authrep"),
                DateTime.Now,
                0,
                true,
                null,
                "trading name",
                Weee.Domain.EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                Weee.Domain.SellingTechniqueType.Both,
                obligationType,
                Weee.Domain.AnnualTurnOverBandType.Greaterthanonemillionpounds,
                brandNames.Select(bn => new BrandName(bn)).ToList(),
                new List<SICCode>(),
                new ChargeBandAmount(Guid.NewGuid(), ChargeBand.A, 123),
                999);
        }
    }
}
