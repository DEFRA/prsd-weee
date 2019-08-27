namespace EA.Weee.DataAccess.DataAccess
{
    using Domain;
    using System.Threading.Tasks;

    public interface ISystemDataDataAccess
    {
        Task<SystemData> Get();
    }
}
