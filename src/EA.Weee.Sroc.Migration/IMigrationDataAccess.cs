namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public interface IMigrationDataAccess
    {
        Task<List<MemberUpload>> Fetch();

        Task Update(Guid id, decimal amount);
    }
}