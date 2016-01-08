namespace EA.Weee.RequestHandlers.Test
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Factories;
    using Prsd.Core.Mediator;
    using Requests.Test;

    public class UpdatePcsSubmissionWindowSettingsHandler : IRequestHandler<UpdatePcsSubmissionWindowSettings, bool>
    {
        private readonly IQuarterFactory quarterFactory;

        public UpdatePcsSubmissionWindowSettingsHandler(IQuarterFactory quarterFactory)
        {
            this.quarterFactory = quarterFactory;
        }

        public async Task<bool> HandleAsync(UpdatePcsSubmissionWindowSettings message)
        {
            if (message.FixCurrentQuarterAndComplianceYear &&
                     (!message.CurrentComplianceYear.HasValue || !message.SelectedQuarter.HasValue))
            {
                throw new InvalidOperationException("A quarter or compliance year cannot be fixed with null values");
            }

            if (message.FixCurrentQuarterAndComplianceYear)
            {
                await quarterFactory.SetFixedQuarter(new Quarter(message.CurrentComplianceYear.Value,
                    (QuarterType)message.SelectedQuarter.Value));
            }

            await quarterFactory.ToggleFixedQuarterUseage(message.FixCurrentQuarterAndComplianceYear);

            return true;
        }
    }
}
