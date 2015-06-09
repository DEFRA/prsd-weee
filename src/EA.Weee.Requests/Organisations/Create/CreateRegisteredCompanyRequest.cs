namespace EA.Weee.Requests.Organisations.Create
{
    using Base;

    public class CreateRegisteredCompanyRequest : CreateOrganisationReqeust
    {
        public string BusinessName { get; set; }

        public string CompanyRegistrationNumber { get; set; }
    }
}
