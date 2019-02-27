namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;

    public interface IAddOffSiteDataAccess
    {
        Task Submit(IEnumerable<Address> address);
    }
}
