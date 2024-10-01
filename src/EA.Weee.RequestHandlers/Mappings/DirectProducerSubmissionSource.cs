namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using EA.Weee.Domain.Producer;

    internal class DirectProducerSubmissionSource
    {
        public DirectRegistrant DirectRegistrant { get; private set; }

        public DirectProducerSubmission DirectProducerSubmission { get; private set; }

        public DirectProducerSubmissionSource(DirectRegistrant directRegistrant,
            DirectProducerSubmission directProducerSubmission)
        {
            Condition.Requires(directRegistrant).IsNotNull();
            Condition.Requires(directProducerSubmission).IsNotNull();

            DirectRegistrant = directRegistrant;
            DirectProducerSubmission = directProducerSubmission;
        }
    }
}
