namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Domain.Scheme;
    using MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;

    public interface IXmlChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Dictionary<string, ProducerCharge> Calculate(ProcessXMLFile message);
    }
}