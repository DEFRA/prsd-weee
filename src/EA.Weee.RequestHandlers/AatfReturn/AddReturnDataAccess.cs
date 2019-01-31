namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;
    
    public class AddReturnDataAccess : IAddReturnDataAccess
    {
        private readonly WeeeContext context;

        public AddReturnDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(Return aatfReturn)
        {
            context.Returns.Add(aatfReturn);

            return context.SaveChangesAsync();
        }
    }
}
