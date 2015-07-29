namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using Domain.PCS;

    public interface IBusinessValidator
    {
        IEnumerable<MemberUploadError> Validate(schemeType deserializedXml, Guid pcsId);
    }
}
