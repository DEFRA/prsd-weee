namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    public class GetSmallProducerDirectRegistrantChargeRequest : IRequest<SmallProducerDirectRegistrantChargeData>
    {
        public int ComplianceYear { get; set; }

        public GetSmallProducerDirectRegistrantChargeRequest(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
