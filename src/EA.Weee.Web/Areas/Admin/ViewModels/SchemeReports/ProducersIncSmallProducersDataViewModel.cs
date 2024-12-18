namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ProducersIncSmallProducersDataViewModel : ProducersDataViewModelBase
    {
        [DisplayName("PCS name or direct registrants")]
        public override Guid? SelectedSchemeId { get; set; }
    }
}