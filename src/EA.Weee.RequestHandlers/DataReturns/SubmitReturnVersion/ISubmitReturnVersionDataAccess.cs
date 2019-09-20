namespace EA.Weee.RequestHandlers.DataReturns.SubmitReturnVersion
{
    using Domain.DataReturns;
    using System.Threading.Tasks;

    public interface ISubmitReturnVersionDataAccess
    {
        Task Submit(DataReturnVersion dataReturnVersion);
    }
}
