﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationMapping : EntityTypeConfiguration<Organisation>
    {
        public OrganisationMapping()
        {
            ToTable("Organisation", "Organisation");

            HasMany(o => o.Schemes).WithRequired(o => o.Organisation).HasForeignKey(o => o.OrganisationId);
        }
    }
}
