namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using Domain.AatfReturn;

    public class AatfSimpleMapObject
    {
        public Aatf Aatf { get; private set; }

        public AatfSimpleMapObject(Aatf aatf)
        {
            Condition.Requires(aatf).IsNotNull();

            Aatf = aatf;
        }
    }
}
