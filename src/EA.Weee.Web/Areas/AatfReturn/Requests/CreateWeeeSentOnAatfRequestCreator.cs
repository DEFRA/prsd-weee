namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class CreateWeeeSentOnAatfRequestCreator : ICreateWeeeSentOnAatfRequestCreator
    {
        public CreateWeeeSentOnAatfSite ViewModelToRequest(CreateWeeeSentOnViewModel viewModel)
        {           
            var aatfSite = new CreateWeeeSentOnAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                SelectedWeeeSentOnId = viewModel.SelectedWeeeSentOnId
            };

            return aatfSite;
        }
    }
}