namespace EA.Prsd.Core.DataAccess.Conventions
{
    using System.Data.Entity.ModelConfiguration.Conventions;

    internal class DecimalPrecision : Convention
    {
        public DecimalPrecision()
        {
            Properties<decimal>()
                .Configure(c =>
                {
                    c.HasPrecision(18, 4);
                });
        }
    }
}