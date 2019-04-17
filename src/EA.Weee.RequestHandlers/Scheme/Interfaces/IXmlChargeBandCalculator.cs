namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Xml.MemberRegistration;

    public interface IXMLChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Dictionary<string, ProducerCharge> Calculate(ProcessXmlFile message);
    }
}