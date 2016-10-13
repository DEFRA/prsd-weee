namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides details about a producer (identified by their registration number)
    /// for a specific compliance year.
    /// </summary>
    public class ProducerDetails
    {
        public bool CanRemoveProducer { get; set; }

        public List<ProducerDetailsScheme> Schemes { get; set; }
    }
}
