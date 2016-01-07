namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;

    public interface IQuarterWindowTemplateDataAccess
    {
        QuarterWindowTemplate GetByQuarter(int quarter);
    }
}
