namespace EA.Weee.RequestHandlers.Test
{
    using System;
    using System.Threading.Tasks;
    using Core.Configuration;
    using Domain.DataReturns;
    using Factories;
    using Prsd.Core.Mediator;
    using Requests.Test;

    public class UpdatePcsSubmissionWindowSettingsHandler : IRequestHandler<UpdatePcsSubmissionWindowSettings, bool>
    {
        private readonly IDateFactory dateFactory;
        private readonly IConfigurationManagerWrapper configurationManagerWrapper;

        public UpdatePcsSubmissionWindowSettingsHandler(IDateFactory dateFactory, IConfigurationManagerWrapper configurationManagerWrapper)
        {
            this.dateFactory = dateFactory;
            this.configurationManagerWrapper = configurationManagerWrapper;
        }

        public async Task<bool> HandleAsync(UpdatePcsSubmissionWindowSettings message)
        {
            if (configurationManagerWrapper.IsLiveEnvironment)
            {
                throw new InvalidOperationException("The current date cannot be fixed in a live environment");
            }

            if (message.FixCurrentDate)
            {
                if (!message.CurrentDate.HasValue)
                {
                    throw new InvalidOperationException("A date cannot be fixed with a null value");
                }

                await dateFactory.SetFixedDate(message.CurrentDate.Value);
            }

            await dateFactory.ToggleFixedDateUsage(message.FixCurrentDate);

            return true;
        }
    }
}
