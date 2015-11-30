namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class SchemeBusiness
    {
        public SchemeCompany Company { get; set; }
        public SchemePartnership Partnership { get; set; }

        public SchemeBusiness()
        {
        }

        public static SchemeBusiness Create(ISettings settings)
        {
            SchemeBusiness schemeBusiness = new SchemeBusiness();

            if (RandomHelper.OneIn(2))
            {
                schemeBusiness.Company = SchemeCompany.Create(settings);
            }
            else
            {
                schemeBusiness.Partnership = SchemePartnership.Create(settings);
            }

            return schemeBusiness;
        }
    }
}
