namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
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

            var model = new SelectYourAatfViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfList = source.AatfList
            };

            return model;
        }
    }
}