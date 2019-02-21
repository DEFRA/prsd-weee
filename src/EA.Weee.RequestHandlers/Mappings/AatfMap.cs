namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Prsd.Core;

    public class AatfMap : IMap<Aatf, AatfData>
    {
        public AatfData Map(Aatf source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new AatfData(source.Id, source.Name, source.ApprovalNumber);
        }
    }
}
