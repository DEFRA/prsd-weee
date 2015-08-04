﻿namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.PCS;

    internal class MemberUploadErrorTypeMapping : ComplexTypeConfiguration<MemberUploadErrorType>
    {
        public MemberUploadErrorTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorType");
        }
    }
}