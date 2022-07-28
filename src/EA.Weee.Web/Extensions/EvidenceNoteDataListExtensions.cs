namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Areas.Aatf.Comparers;
    using Core.AatfEvidence;
    using Core.Scheme;

    public static class EvidenceNoteDataListExtensions
    {
        public static List<OrganisationSchemeData> CreateOrganisationSchemeDataList(this List<EvidenceNoteData> list)
        {
            return list.Select(x =>
                    new OrganisationSchemeData() { DisplayName = x.RecipientOrganisationData.IsBalancingScheme ? x.RecipientOrganisationData.OrganisationName : x.RecipientSchemeData.SchemeName, Id = x.RecipientOrganisationData.Id })
                .Distinct(new SchemeOrganisationDataComparer())
                .OrderBy(s => s.DisplayName)
                .ToList();
        }
    }
}