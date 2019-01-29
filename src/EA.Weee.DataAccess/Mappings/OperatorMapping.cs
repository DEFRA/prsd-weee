﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class OperatorMapping : EntityTypeConfiguration<Operator>
    {
        public OperatorMapping()
        {
            ToTable("Operator", "AATF");
        }
    }
}
