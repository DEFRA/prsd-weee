namespace EA.Weee.RequestHandlers.DataReturns
{
    using Domain.DataReturns;
    using Prsd.Core;

    public class DataReturnVersionComparer : IDataReturnVersionComparer
    {
        public bool EeeDataChanged(DataReturnVersion currentSubmission, DataReturnVersion previousSubmission)
        {
            Guard.ArgumentNotNull(() => currentSubmission, currentSubmission);

            bool result = false;

            if (previousSubmission != null)
            {
                if (currentSubmission.EeeOutputReturnVersion != null)
                {
                    result =
                        previousSubmission.EeeOutputReturnVersion == null ||
                        currentSubmission.EeeOutputReturnVersion.Id != previousSubmission.EeeOutputReturnVersion.Id;
                }
                else if (previousSubmission.EeeOutputReturnVersion != null)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
