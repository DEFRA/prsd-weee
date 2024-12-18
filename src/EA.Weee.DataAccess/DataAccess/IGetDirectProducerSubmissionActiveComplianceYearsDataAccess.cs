namespace EA.Weee.DataAccess.DataAccess
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetDirectProducerSubmissionActiveComplianceYearsDataAccess
    {
        Task<List<int>> Get(int offSet);
    }
}