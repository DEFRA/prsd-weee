namespace EA.Weee.Core.AatfReturn
{
    using EA.Prsd.Core;
    public class AatfSchemeData
    {
        public Scheme Scheme { get; private set; }

        public ObligatedCategoryValue Received { get; private set; }

        public string ApprovalName { get; set; }

        public AatfSchemeData(Scheme scheme, ObligatedCategoryValue received, string approvalName)
        {
            // GUARD AGAINST Scheme and ObligatedCategoryValue BEING NULL AND CREATE UNIT TEST TO TEST THE GUARD
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => received, received);            

            Scheme = scheme;
            Received = received;

            ApprovalName = approvalName;
        }
    }
}
