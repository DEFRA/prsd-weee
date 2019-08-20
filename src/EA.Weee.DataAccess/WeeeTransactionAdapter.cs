namespace EA.Weee.DataAccess
{
    using System.Data.Entity;

    public class WeeeTransactionAdapter : IWeeeTransactionAdapter
    {
        private readonly WeeeContext context;

        public WeeeTransactionAdapter(WeeeContext context)
        {
            this.context = context;
        }

        public virtual DbContextTransaction BeginTransaction()
        {
            return context.Database.BeginTransaction();
        }

        public virtual void Dispose(DbContextTransaction transaction)
        {
            transaction.Dispose();
        }

        public virtual void Commit(DbContextTransaction transaction)
        {
            transaction.Commit();
        }

        public virtual void Rollback(DbContextTransaction transaction)
        {
            transaction.Rollback();
        }
    }
}
