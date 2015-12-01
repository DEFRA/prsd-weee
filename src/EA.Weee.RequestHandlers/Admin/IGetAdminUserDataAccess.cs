namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Threading.Tasks;
 
    public interface IGetAdminUserDataAccess
    {
        Task<Domain.Admin.CompetentAuthorityUser> GetAdminUserOrDefault(Guid userId);
    }
}
