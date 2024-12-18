namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Weee.Core.DirectRegistrant;

    public class SmallProducerSubmissionMapperData
    {
        public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

        public bool? RedirectToCheckAnswers { get; set; }

        // if ture the high level organisation data will be used rather than the current in progress data
        public bool UseMasterVersion { get; set; }
        public int? Year { get; set; }
    }
}