namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;

    public interface IProducerChargeCalculatorDataAccess
    {
        ChargeBandAmount FetchCurrentChargeBandAmount(ChargeBand chargeBandType);

        decimal FetchSumOfExistingCharges(string schemeApprovalNumber, string registrationNumber, int complianceYear);
    }
}
