namespace EA.Prsd.Core.DataAccess.Conventions
{
    using System.Data.Entity.ModelConfiguration.Conventions;

    internal class IdIsPrimaryKey : Convention
    {
        public IdIsPrimaryKey()
        {
            Properties()
                .Where(p => p.Name == "Id")
                .Configure(c =>
                {
                    c.IsKey();
                    c.IsRequired();
                });
        }
    }
}