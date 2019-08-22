namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQuarterWindowTemplateDataAccess
    {
        Task<QuarterWindowTemplate> GetByQuarter(int quarter);

        Task<List<QuarterWindowTemplate>> GetAll();
    }
}
