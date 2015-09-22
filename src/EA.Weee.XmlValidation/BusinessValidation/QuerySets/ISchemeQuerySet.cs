namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using Domain.Scheme;

    public interface ISchemeQuerySet
    {
        Scheme GetScheme(Guid schemeId);
    }
}
