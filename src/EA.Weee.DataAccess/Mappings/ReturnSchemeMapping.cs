namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class ReturnSchemeMapping : EntityTypeConfiguration<ReturnScheme>
    {
        public ReturnSchemeMapping()
        {
            ToTable("ReturnScheme", "AATF");
        }
    }
}
