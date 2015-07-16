namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.DataValidation
{
    using System.Collections.Generic;
    using Domain.PCS;

    public interface IDataValidator
    {
        IEnumerable<MemberUploadError> Validate(schemeType deserializedXml);
    }
}
