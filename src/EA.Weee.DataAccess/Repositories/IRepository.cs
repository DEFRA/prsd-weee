namespace EA.Weee.DataAccess.Repositories
{
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task<int> SaveChangesAsync();
    }
}
