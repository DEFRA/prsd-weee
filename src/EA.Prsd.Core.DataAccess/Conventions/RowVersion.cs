namespace EA.Prsd.Core.DataAccess.Conventions
{
    using System.Data.Entity.ModelConfiguration.Conventions;

    internal class RowVersion : Convention
    {
        public RowVersion()
        {
            Properties()
                .Where(p => p.Name == "RowVersion")
                .Configure(c =>
                {
                    c.IsRowVersion();
                    c.IsRequired();
                });
        }
    }
}