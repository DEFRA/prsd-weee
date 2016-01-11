namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core.Domain;

    public class SystemData : Entity
    {
        public long LatestPrnSeed { get; set; }

        public ulong InitialIbisFileId { get; private set; }

        public bool UseFixedCurrentDate { get; private set; }

        public DateTime FixedCurrentDate { get; private set; }

        protected SystemData()
        {
        }

        public void UpdateFixedCurrentDate(DateTime date)
        {
            FixedCurrentDate = date;
        }

        public void ToggleFixedCurrentDateUsage(bool enabled)
        {
            UseFixedCurrentDate = enabled;
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
