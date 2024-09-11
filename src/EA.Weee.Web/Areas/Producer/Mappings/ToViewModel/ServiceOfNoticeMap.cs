namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class ServiceOfNoticeMap : IMap<SmallProducerSubmissionData, ServiceOfNoticeViewModel>
    {
        public ServiceOfNoticeViewModel Map(SmallProducerSubmissionData source)
        {
            return new ServiceOfNoticeViewModel();
        }
    }
}