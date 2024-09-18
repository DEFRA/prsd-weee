﻿namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class PaymentStatusMapping : ComplexTypeConfiguration<PaymentStatus>
    {
        public PaymentStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
