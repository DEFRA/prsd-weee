namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using EA.Weee.Core.Admin;

    /// <summary>
    /// Note: For now this class is a wrapper for the model returned by the API.
    /// It is here in case the details page becomes interactive in the future,
    /// for example, if it allows a compliance year to be selected.
    /// </summary>
    public class DetailsViewModel
    {
        public ProducerDetails Details { get; set; }
    }
}