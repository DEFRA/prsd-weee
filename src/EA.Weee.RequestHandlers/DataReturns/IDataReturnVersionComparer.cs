namespace EA.Weee.RequestHandlers.DataReturns
{
    using Domain.DataReturns;

    public interface IDataReturnVersionComparer
    {
        bool EeeDataChanged(DataReturnVersion currentSubmission, DataReturnVersion previousSubmission);
    }
}