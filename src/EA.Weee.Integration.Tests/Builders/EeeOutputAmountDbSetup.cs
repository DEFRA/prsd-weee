namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System;

    public class EeeOutputAmountDbSetup : DbTestDataBuilder<EeeOutputAmount, EeeOutputAmountDbSetup>
    {
        protected override EeeOutputAmount Instantiate()
        {
            instance = new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, 0,
                new RegisteredProducer("reg", SystemTime.UtcNow.Year));

            return instance;
        }

        public EeeOutputAmountDbSetup WithData(Guid registeredProducerId, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            ObjectInstantiator<EeeOutputAmount>.SetProperty(o => o.RegisteredProducer, null, instance);
            ObjectInstantiator<EeeOutputAmount>.SetProperty(o => o.RegisteredProducerId, registeredProducerId, instance);
            ObjectInstantiator<EeeOutputAmount>.SetProperty(o => o.WeeeCategory, category, instance);
            ObjectInstantiator<EeeOutputAmount>.SetProperty(o => o.Tonnage, tonnage, instance);
            ObjectInstantiator<EeeOutputAmount>.SetProperty(o => o.ObligationType, obligationType, instance);
            return this;
        }
    }
}
