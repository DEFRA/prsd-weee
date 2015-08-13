namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeStatusMap : IMap<SchemeStatus, Core.Shared.SchemeStatus>
    {
        public Core.Shared.SchemeStatus Map(SchemeStatus source)
        {
            if (source == SchemeStatus.Approved)
            {
                return Core.Shared.SchemeStatus.Approved;
            }

            if (source == SchemeStatus.Rejected)
            {
                return Core.Shared.SchemeStatus.Rejected;
            }

            return Core.Shared.SchemeStatus.Pending;
        }
    }
}
