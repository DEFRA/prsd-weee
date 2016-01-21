namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;

    public class ObligationTypeMap : IMap<ObligationType, Core.Shared.ObligationType>
    {
        public Core.Shared.ObligationType Map(ObligationType source)
        {
            return (Core.Shared.ObligationType)(int)source;
        }
    }
}
