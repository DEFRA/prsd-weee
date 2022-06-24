namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AatfEvidenceSelectYourAatfViewModelMap : IMap<AatfEvidenceToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel>
    {
        public SelectYourAatfViewModel Map(AatfEvidenceToSelectYourAatfViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var aatfList = new List<AatfData>();

            if (source.CurrentDate.Date >= source.EvidenceSiteSelectionStartDateFrom && source.AatfList != null)
            {
                aatfList = source.AatfList
                    .Where(a => a.EvidenceSiteDisplay)
                    .GroupBy(a => a.AatfId)
                    .Select(a => a.OrderByDescending(a1 => a1.ComplianceYear).First())
                    .OrderBy(a => a.Name)
                    .ToList();
            }

            foreach (var aatfData in aatfList)
            {
                aatfData.Name = $"{aatfData.Name} ({aatfData.ApprovalNumber})";
            }

            var model = new SelectYourAatfViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfList = aatfList
            };

            return model;
        }
    }
}