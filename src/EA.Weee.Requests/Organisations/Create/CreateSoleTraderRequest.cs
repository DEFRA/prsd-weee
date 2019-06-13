namespace EA.Weee.Requests.Organisations.Create
{
    using Base;

    public class CreateSoleTraderRequest : CreateOrganisationRequest
    {
        public string BusinessName { get; set; }
    }
}
