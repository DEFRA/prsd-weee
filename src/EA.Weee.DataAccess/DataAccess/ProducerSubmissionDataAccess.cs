namespace EA.Weee.DataAccess.DataAccess
{
    using System.Collections.Generic;
    using Domain.Producer;

    public class ProducerSubmissionDataAccess : IProducerSubmissionDataAccess
    {
        private readonly WeeeContext context;

        public ProducerSubmissionDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public void AddRange(IEnumerable<ProducerSubmission> producerSubmissions)
        {
            context.AllProducerSubmissions.AddRange(producerSubmissions);
        }
    }
}
