namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ReturnSchemeMapping : EntityTypeConfiguration<ReturnScheme>
    {
        public ReturnSchemeMapping()
        {
            ToTable("ReturnScheme", "AATF");
        }
    }
}
