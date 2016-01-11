namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using ReturnVersionBuilder;

    public interface ISubmissionWindowClosed
    {
        Task<DataReturnVersionBuilderResult> Validate(Quarter quarter);
    }
}
