namespace EA.Weee.RequestHandlers.DataReturns.SubmitReturnVersion
{
    using DataAccess;
    using Domain.DataReturns;
    using System.Threading.Tasks;

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
