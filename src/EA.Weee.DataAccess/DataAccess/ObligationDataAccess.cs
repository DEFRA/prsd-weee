namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
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
            IList<ObligationUploadError> errors)
        {
            var obligationUpload = new ObligationUpload(ukCompetentAuthority,
                userContext.UserId.ToString(), data, fileName, errors);

            var updatedObligation = await genericDataAccess.Add(obligationUpload);

            await context.SaveChangesAsync();

            return updatedObligation.Id;
        }
    }
}
