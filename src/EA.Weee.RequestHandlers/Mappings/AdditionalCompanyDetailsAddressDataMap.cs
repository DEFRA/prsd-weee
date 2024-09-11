namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Domain.Organisation;
    using System.Linq;
    using CuttingEdge.Conditions;

    public class AdditionalCompanyDetailsAddressDataMap : IMap<ICollection<AdditionalCompanyDetails>, IList<AdditionalCompanyDetailsData>>
    {
        public IList<AdditionalCompanyDetailsData> Map(ICollection<AdditionalCompanyDetails> source)
        {
            Condition.Requires(source).IsNotNull();

            if (!source.Any())
            {
                return new List<AdditionalCompanyDetailsData>();
            }

            return source.Select(s => new AdditionalCompanyDetailsData()
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList();
        }
    }
}
