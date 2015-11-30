namespace EA.Weee.Requests.Organisations.Create
{
    using Base;

    public class CreateRegisteredCompanyRequest : CreateOrganisationRequest
    {
        public string BusinessName { get; set; }

        public string CompanyRegistrationNumber { get; set; }
    }
}
