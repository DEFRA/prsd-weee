namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Domain.Scheme;

    public interface IBusinessValidator
    {
        IEnumerable<MemberUploadError> Validate(schemeType deserializedXml, Guid pcsId);
    }
}
