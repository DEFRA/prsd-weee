namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class SystemData : Entity
    {
        public long LatestPrnSeed { get; set; }

        public ulong InitialIbisFileId { get; private set; }

        protected SystemData()
        {
        }

        /// <summary>
        /// This property is used by Entity Framework to map the ulong InitialIbisFileId property
        /// to the BIGINT column of the database.
        /// </summary>
        public long InitialIbisFileIdDatabaseValue
        {
            get { return (long)InitialIbisFileId; }
            private set { InitialIbisFileId = (ulong)value; }
        }
    }
}
