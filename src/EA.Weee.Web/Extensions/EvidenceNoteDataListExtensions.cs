namespace EA.Weee.Web.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Areas.Aatf.Comparers;
    using Core.AatfEvidence;
    using EA.Weee.Core.Shared;

    public static class EvidenceNoteDataListExtensions
    {
        public static List<EntityIdDisplayNameData> CreateOrganisationSchemeDataList(this List<EvidenceNoteData> list)
        {
            return list.Select(x =>
                    new EntityIdDisplayNameData() { DisplayName = x.RecipientOrganisationData.IsBalancingScheme ? x.RecipientOrganisationData.OrganisationName : x.RecipientSchemeData.SchemeName, Id = x.RecipientOrganisationData.Id })
                .Distinct(new SchemeOrganisationDataComparer())
                .OrderBy(s => s.DisplayName)
                .ToList();
        }
    }
}