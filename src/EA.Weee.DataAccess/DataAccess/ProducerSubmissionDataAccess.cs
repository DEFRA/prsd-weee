namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Producer;
    using System.Collections.Generic;

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
