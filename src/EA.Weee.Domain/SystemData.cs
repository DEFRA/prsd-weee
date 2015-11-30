namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class SystemData : Entity
    {
        public long LatestPrnSeed { get; set; }

        protected SystemData()
        {
        }
    }
}
