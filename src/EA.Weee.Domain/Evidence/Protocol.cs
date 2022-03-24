namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class Protocol : Enumeration
    {
        public static readonly Protocol Actual = new Protocol(1, "Actual");
        public static readonly Protocol NationalProtocol = new Protocol(2, "National protocol");
        public static readonly Protocol SiteProtocol = new Protocol(2, "Site protocol");
        public static readonly Protocol ReuseNetworkWeights = new Protocol(2, "Re-use network weights");

        protected Protocol()
        {
        }

        private Protocol(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
