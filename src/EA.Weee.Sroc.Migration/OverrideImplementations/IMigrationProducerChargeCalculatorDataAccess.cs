namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using RequestHandlers.Scheme.MemberRegistration;

    public interface IMigrationProducerChargeCalculatorDataAccess : IProducerChargeCalculatorDataAccess
    {
        decimal FetchSumOfExistingChargesByDate(string schemeApprovalNumber, string registrationNumber, int complianceYear, DateTime createdDate);
    }
}
