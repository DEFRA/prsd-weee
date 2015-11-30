namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class ProducerBusiness
    {
        public CorrespondentForNotices CorrespondentForNotices { get; private set; }
        public Partnership Partnership { get; set; }
        public Company Company { get; set; }

        public ProducerBusiness()
        {
            CorrespondentForNotices = new CorrespondentForNotices();
        }

        public static ProducerBusiness Create(ISettings settings, bool noCompany)
        {
            ProducerBusiness producerBusiness = new ProducerBusiness();
            
            producerBusiness.CorrespondentForNotices = CorrespondentForNotices.Create(settings);

            if (noCompany || RandomHelper.OneIn(2))
            {
                producerBusiness.Partnership = Partnership.Create(settings);
            }
            else
            {
                producerBusiness.Company = Company.Create(settings);
            }

            return producerBusiness;
        }
    }
}
