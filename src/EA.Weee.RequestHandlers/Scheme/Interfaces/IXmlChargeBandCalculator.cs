namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections;
    using System.Collections.Generic;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;

    public interface IXmlChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Hashtable Calculate(ProcessXMLFile message);
    }
}