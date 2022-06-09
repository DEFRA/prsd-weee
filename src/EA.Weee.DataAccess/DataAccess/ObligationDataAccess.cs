namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Obligation;
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

            if (!errors.Any())
            {
                obligationUpload.SetObligations(obligations);
            }
            else
            {
                obligationUpload.SetErrors(errors);
            }
            
            var updatedObligation = await genericDataAccess.Add(obligationUpload);
            
            return updatedObligation.Id;
        }
    }
}
