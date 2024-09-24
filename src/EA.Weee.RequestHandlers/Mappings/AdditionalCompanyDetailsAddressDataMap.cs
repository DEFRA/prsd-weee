namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.Organisation;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;

    public class AdditionalCompanyDetailsAddressDataMap : IMap<ICollection<AdditionalCompanyDetails>, IList<AdditionalCompanyDetailsData>>
    {
        public IList<AdditionalCompanyDetailsData> Map(ICollection<AdditionalCompanyDetails> source)
        {
            Condition.Requires(source).IsNotNull();

            if (!source.Any())
            {
                return new List<AdditionalCompanyDetailsData>();
            }

            return source.OrderBy(a => a.Order).Select(s => new AdditionalCompanyDetailsData()
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList();
        }
    }
}
