namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;

    public class AddressType : Enumeration
    {
        public static readonly AddressType OrganisationAddress = new AddressType(1, "Organisation address");
        public static readonly AddressType RegisteredOrPPBAddress = new AddressType(2, "Registered or PPB address");
        public static readonly AddressType SercviceOfNoticeAddress = new AddressType(3, "Service of notice address");

        protected AddressType()
        {
        }

        private AddressType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
