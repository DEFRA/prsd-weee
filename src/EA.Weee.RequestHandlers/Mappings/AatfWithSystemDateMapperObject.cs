namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;
    using Domain.AatfReturn;

    public class AatfWithSystemDateMapperObject
    {
        public Aatf Aatf { get; private set; }

        public DateTime SystemDateTime { get; private set; }

        public AatfWithSystemDateMapperObject(Aatf aatf, DateTime systemDateTime)
        {
            Condition.Requires(aatf).IsNotNull();
            Condition.Requires(systemDateTime).IsNotEqualTo(default);

            Aatf = aatf;
            SystemDateTime = systemDateTime;
        }
    }
}
