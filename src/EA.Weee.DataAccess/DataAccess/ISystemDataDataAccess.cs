namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using Domain;
    using System.Threading.Tasks;

    public interface ISystemDataDataAccess
    {
        Task<SystemData> Get();

        Task<DateTime> GetSystemDateTime();
    }
}
