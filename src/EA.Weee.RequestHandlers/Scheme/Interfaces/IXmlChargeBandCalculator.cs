namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using Xml.MemberRegistration;

    public interface IXMLChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Dictionary<string, ProducerCharge> Calculate(ProcessXmlFile message);
    }
}