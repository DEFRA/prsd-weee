namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain.Producer;
    using System.Collections.Generic;

    public interface IMigratedProducerQuerySet
    {
        MigratedProducer GetMigratedProducer(string registrationNo);

        List<string> GetAllRegistrationNumbers();

        List<MigratedProducer> GetAllMigratedProducers();
    }
}
