namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Xml.MemberRegistration;

    public interface IMigrationChargeBandCalculator
    {
        Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer, MemberUpload memberUpload);

        bool IsMatch(schemeType scheme, producerType producer, MemberUpload upload, string name);
    }
}
