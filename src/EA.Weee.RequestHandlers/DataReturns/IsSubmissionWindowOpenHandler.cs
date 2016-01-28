namespace EA.Weee.RequestHandlers.DataReturns
{   
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Factories;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    
    internal class IsSubmissionWindowOpenHandler : IRequestHandler<IsSubmissionWindowOpen, bool>
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public IsSubmissionWindowOpenHandler(IQuarterWindowFactory quarterWindowFactory, ISystemDataDataAccess systemDataDataAccess)
        {
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<bool> HandleAsync(IsSubmissionWindowOpen query)
        {
            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var possibleQuarterWindows = await quarterWindowFactory.GetPossibleQuarterWindow(currentDate.Year);

            foreach (var possibleQuarterWindow in possibleQuarterWindows)
            {
                if (currentDate >= possibleQuarterWindow.StartDate && currentDate <= possibleQuarterWindow.EndDate)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
