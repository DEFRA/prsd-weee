namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnAndAatfToSentOnCreateWeeeSentOnViewModelMap : IMap<ReturnAndAatfToSentOnCreateWeeeSentOnViewModelMapTransfer, CreateWeeeSentOnViewModel>
    {
        public CreateWeeeSentOnViewModel Map(ReturnAndAatfToSentOnCreateWeeeSentOnViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new CreateWeeeSentOnViewModel()
            {
                SelectedWeeeSentOnId = source.SelectedWeeeSentOnId,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                WeeeSenOnId = source.WeeeSentOnId.Value
            };

            return viewModel;
        }
    }
}