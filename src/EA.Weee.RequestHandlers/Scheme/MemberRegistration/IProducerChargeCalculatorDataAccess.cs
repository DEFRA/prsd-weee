namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IProducerChargeCalculatorDataAccess
    {
        decimal FetchChargeBandAmount(ChargeBandType chargeBand);
        decimal FetchSumOfExistingCharges(string registrationNumber, int complianceYear);
    }
}
