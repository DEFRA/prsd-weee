namespace EA.Weee.DataAccess.DataAccess
{
    using System.Threading.Tasks;
    using Domain;

    public interface ISystemDataDataAccess
    {
        Task<SystemData> Get();
    }
}
