namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class DataReturnVersionBuilderDataAccess : IDataReturnVersionBuilderDataAccess
    {
        private readonly WeeeContext context;

        public DataReturnVersionBuilderDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DataReturn> FetchDataReturnOrDefaultAsync(Domain.Scheme.Scheme scheme, Quarter quarter)
        {
            return await context.DataReturns
                .Where(dr => dr.Scheme.Id == scheme.Id)
                .Where(dr => dr.Quarter.Year == quarter.Year)
                .Where(dr => dr.Quarter.Q == quarter.Q)
                .SingleOrDefaultAsync();
        }
    }
}
