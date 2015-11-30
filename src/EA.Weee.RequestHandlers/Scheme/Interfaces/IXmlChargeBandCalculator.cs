namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using MemberRegistration;
    using Requests.Scheme.MemberRegistration;

    public interface IXmlChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Dictionary<string, ProducerCharge> Calculate(ProcessXMLFile message);
    }
}