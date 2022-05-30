namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using Prsd.Core.Mapper;
    using ViewModels.Obligations;

    public class UploadObligationsViewModelMap : IMap<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>
    {
        public UploadObligationsViewModel Map(UploadObligationsViewModelMapTransfer source)
        {
            Condition.Requires(source).IsNotNull();

            throw new System.NotImplementedException();
        }
    }
}