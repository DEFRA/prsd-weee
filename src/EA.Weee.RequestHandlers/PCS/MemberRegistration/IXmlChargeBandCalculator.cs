namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Collections;
    using System.Collections.Generic;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;

    public interface IXmlChargeBandCalculator
    {
        List<MemberUploadError> ErrorsAndWarnings { get; set; }

        Hashtable Calculate(ProcessXMLFile message);
    }
}