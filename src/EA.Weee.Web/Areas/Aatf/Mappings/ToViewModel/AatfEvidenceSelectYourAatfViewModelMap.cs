namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AatfEvidenceSelectYourAatfViewModelMap : IMap<AatfEvidenceToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel>
    {
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;

        public AatfEvidenceSelectYourAatfViewModelMap(IAatfEvidenceHelper aatfEvidenceHelper)
        {
            this.aatfEvidenceHelper = aatfEvidenceHelper;
        }

        public SelectYourAatfViewModel Map(AatfEvidenceToSelectYourAatfViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            var aatfList = new List<AatfData>();

            if (source.AatfList != null)
            {
                aatfList = aatfEvidenceHelper.GroupedValidAatfs(source.AatfList);
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