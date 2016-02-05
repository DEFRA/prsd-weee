namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public class ConfirmWithdrawnViewModel : RadioButtonStringCollectionViewModel
    {
        public ConfirmWithdrawnViewModel() : base(new List<string> { ConfirmSchemeWithdrawOptions.Yes, ConfirmSchemeWithdrawOptions.No })
        {
        }
    }
}