namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberUpload;

    public interface IProducerChargeCalculator
    {
        ProducerCharge CalculateCharge(string schemeApprovalNumber, producerType producer, int complianceYear);
    }
}
