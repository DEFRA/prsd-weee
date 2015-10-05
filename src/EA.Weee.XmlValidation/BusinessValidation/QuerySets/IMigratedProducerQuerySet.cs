namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System.Collections.Generic;
    using Domain.Producer;

    public interface IMigratedProducerQuerySet
    {
        MigratedProducer GetMigratedProducer(string registrationNo);

        List<string> GetAllRegistrationNumbers();

        List<MigratedProducer> GetAllMigratedProducers();
    }
}
