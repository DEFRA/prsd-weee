namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using Domain.Producer;

    public interface IValidationContext
    {
        IEnumerable<Producer> Producers { get; }

        IEnumerable<MigratedProducer> MigratedProducers { get; }
    }
}
