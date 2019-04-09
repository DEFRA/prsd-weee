namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using RequestHandlers.Scheme.MemberRegistration;

    public interface IMigrationProducerChargeCalculatorDataAccess
    {
        ChargeBandAmount FetchCurrentChargeBandAmount(ChargeBand chargeBandType);
    }
}
