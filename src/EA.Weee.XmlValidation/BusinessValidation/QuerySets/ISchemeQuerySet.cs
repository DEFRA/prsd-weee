namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;

    public interface ISchemeQuerySet
    {
        string GetSchemeApprovalNumber(Guid schemeId);
    }
}
