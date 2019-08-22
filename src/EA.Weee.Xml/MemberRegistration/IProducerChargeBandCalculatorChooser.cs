﻿namespace EA.Weee.Xml.MemberRegistration
{
    using System.Threading.Tasks;

    public interface IProducerChargeBandCalculatorChooser
    {
        Task<ProducerCharge> GetProducerChargeBand(schemeType scheme, producerType producer);
    }
}