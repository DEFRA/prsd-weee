namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using RequestHandlers.Scheme.MemberRegistration;
    using Xml.MemberRegistration;

    public interface IMigrationProducerChargeCalculator : IProducerChargeCalculator
    {      
        ProducerCharge CalculateCharge(schemeType scheme, producerType producer, int complianceYear, DateTime submittedDate);
    }
}
