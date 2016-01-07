namespace EA.Weee.DataAccess.DataAccess
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public interface IQuarterWindowTemplateDataAccess
    {
        Task<QuarterWindowTemplate> GetByQuarter(int quarter);
    }
}
