namespace EA.Weee.Core.AatfReturn
{
    public class AatfSchemeData
    {
        public Scheme Scheme { get; private set; }

        public ObligatedCategoryValue Received { get; private set; }

        public AatfSchemeData(Scheme scheme, ObligatedCategoryValue received)
        {
            // GUARD AGAINST Scheme and ObligatedCategoryValue BEING NULL AND CREATE UNIT TEST TO TEST THE GUARD
            Scheme = scheme;
            Received = received;
        }
    }
}
