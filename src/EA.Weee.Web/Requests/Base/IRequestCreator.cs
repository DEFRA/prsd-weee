namespace EA.Weee.Web.Requests.Base
{
    public interface IRequestCreator<in TViewModel, out TRequest>
    {
        TRequest ViewModelToRequest(TViewModel viewModel);
    }
}
