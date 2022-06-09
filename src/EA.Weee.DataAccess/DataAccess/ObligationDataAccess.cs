namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Obligation;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class ObligationDataAccess : IObligationDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;

        public ObligationDataAccess(WeeeContext context, 
            IUserContext userContext, 
            IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.userContext = userContext;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<Guid> AddObligationUpload(UKCompetentAuthority ukCompetentAuthority,
            string data, 
            string fileName,
            IList<ObligationUploadError> errors,
            IList<ObligationScheme> obligations)
        {
            var obligationUpload = new ObligationUpload(ukCompetentAuthority, userContext.UserId.ToString(), data, fileName);

            foreach (var obligationScheme in obligations)
            {
                obligationScheme.SetUpdatedDate(SystemTime.UtcNow, obligationScheme.Obligation);
            }

            if (!errors.Any())
            {
                obligationUpload.SetObligations(obligations);
            }
            else
            {
                obligationUpload.SetErrors(errors);
            }

            //retrieve the current list of obligation scheme values 
            //if none exist then create new values
            //other wise update the existing values by adding them to the current obligation upload?
            var updatedObligation = await genericDataAccess.Add(obligationUpload);
            
            return updatedObligation.Id;
        }
    }
}
