namespace EA.Weee.Core.AatfReturn
{
    using Core.Scheme;
    using EA.Prsd.Core;
    public class AatfSchemeData
    {
        public SchemeData Scheme { get; private set; }

        public ObligatedCategoryValue Received { get; private set; }

        public string ApprovalName { get; set; }

        public AatfSchemeData(SchemeData scheme, ObligatedCategoryValue received, string approvalName)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => received, received);            

            Scheme = scheme;
            Received = received;

            ApprovalName = approvalName;
        }
    }
}
