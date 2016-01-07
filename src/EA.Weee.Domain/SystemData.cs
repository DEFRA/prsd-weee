namespace EA.Weee.Domain
{
    using DataReturns;
    using Prsd.Core.Domain;

    public class SystemData : Entity
    {
        public long LatestPrnSeed { get; set; }

        public ulong InitialIbisFileId { get; private set; }

        public bool UseSetComplianceYearAndQuarter { get; private set; }

        public int SetComplianceYear { get; private set; }

        public QuarterType SetQuarter { get; private set; }

        protected SystemData()
        {
        }

        public void UpdateQuarterAndComplianceYear(Quarter quarter)
        {
            SetComplianceYear = quarter.Year;
            SetQuarter = quarter.Q;
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
