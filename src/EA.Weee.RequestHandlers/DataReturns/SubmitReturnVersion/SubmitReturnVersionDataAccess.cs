namespace EA.Weee.RequestHandlers.DataReturns.SubmitReturnVersion
{
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;

    public class SubmitReturnVersionDataAccess : ISubmitReturnVersionDataAccess
    {
        private readonly WeeeContext context;

        public SubmitReturnVersionDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(DataReturnVersion dataReturnVersion)
        {
            dataReturnVersion.Submit(context.GetCurrentUser());

            return context.SaveChangesAsync();
        }
    }
}
