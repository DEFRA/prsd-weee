namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAddReturnDataAccess
    {
        Task Submit(Return aatfReturn);
    }
}
