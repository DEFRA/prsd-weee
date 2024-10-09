namespace EA.Weee.Requests.Admin.DirectRegistrants
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    public class GetSmallProducerSubmissionByRegistrationNumber : IRequest<SmallProducerSubmissionData>
    {
        public string RegistrationNumber {get; set; }

        public GetSmallProducerSubmissionByRegistrationNumber(string registrationNumber)
        {
            Condition.Requires(registrationNumber).IsNotNullOrWhiteSpace();

            RegistrationNumber = registrationNumber;
        }
    }
}
