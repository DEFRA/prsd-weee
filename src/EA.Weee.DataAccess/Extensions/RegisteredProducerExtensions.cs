namespace EA.Weee.DataAccess.Extensions
{
    using System.Data.Entity;
    using System.Linq;
    using Domain.Producer;

    public static class RegisteredProducerExtensions
    {
        public static IQueryable<RegisteredProducer> OnlyAlignedRegisteredProducers(
            this IQueryable<RegisteredProducer> registeredProducers)
        {
            return registeredProducers.Where(rp => rp.IsAligned);
        }

        public static IQueryable<RegisteredProducer> OnlyUnalignedRegisteredProducers(
            this IQueryable<RegisteredProducer> registeredProducers)
        {
            return registeredProducers.Where(rp => !rp.IsAligned);
        }
    }
}
