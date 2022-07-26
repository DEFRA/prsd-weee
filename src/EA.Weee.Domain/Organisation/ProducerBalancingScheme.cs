namespace EA.Weee.Domain.Organisation
{
    public class ProducerBalancingScheme
    {
        public string Lock { get; set; }

        public virtual Organisation Organisation { get; private set; }

        public ProducerBalancingScheme()
        {
        }
    }
}
