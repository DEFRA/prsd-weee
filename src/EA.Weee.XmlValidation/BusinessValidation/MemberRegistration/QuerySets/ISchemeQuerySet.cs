namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System;

    public interface ISchemeQuerySet
    {
        string GetSchemeApprovalNumber(Guid schemeId);
    }
}
