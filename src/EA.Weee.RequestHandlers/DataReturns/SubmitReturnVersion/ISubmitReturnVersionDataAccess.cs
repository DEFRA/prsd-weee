namespace EA.Weee.RequestHandlers.DataReturns.SubmitReturnVersion
{
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface ISubmitReturnVersionDataAccess
    {
        Task Submit(DataReturnVersion dataReturnVersion);
    }
}
