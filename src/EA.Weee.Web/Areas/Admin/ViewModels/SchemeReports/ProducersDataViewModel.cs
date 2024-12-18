namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System;
    using System.ComponentModel;

    public class ProducersDataViewModel : ProducersDataViewModelBase
    {
        [DisplayName("PCS name")]
        public new Guid? SelectedSchemeId { get; set; }
    }
}