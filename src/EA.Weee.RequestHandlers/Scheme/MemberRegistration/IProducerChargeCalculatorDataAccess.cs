namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IProducerChargeCalculatorDataAccess
    {
        ChargeBandAmount FetchCurrentChargeBandAmount(ChargeBand chargeBandType);

        decimal FetchSumOfExistingCharges(string schemeApprovalNumber, string registrationNumber, int complianceYear);
    }
}
