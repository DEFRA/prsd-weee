﻿namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;

    public class DirectProducerSubmissionStatus : Enumeration
    {
        public static readonly DirectProducerSubmissionStatus Incomplete = new DirectProducerSubmissionStatus(1, "Incomplete");
        public static readonly DirectProducerSubmissionStatus Complete = new DirectProducerSubmissionStatus(2, "Submitted");
        public static readonly DirectProducerSubmissionStatus Returned = new DirectProducerSubmissionStatus(3, "Returned");

        protected DirectProducerSubmissionStatus()
        {
        }

        private DirectProducerSubmissionStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
