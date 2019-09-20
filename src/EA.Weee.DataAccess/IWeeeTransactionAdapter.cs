namespace EA.Weee.DataAccess
{
    using System.Data.Entity;

    public interface IWeeeTransactionAdapter
    {
        DbContextTransaction BeginTransaction();

        void Dispose(DbContextTransaction transaction);

        void Commit(DbContextTransaction transaction);

        void Rollback(DbContextTransaction transaction);
    }
}