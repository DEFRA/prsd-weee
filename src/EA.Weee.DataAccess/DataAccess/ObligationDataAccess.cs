namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Obligation;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Z.EntityFramework.Plus;

    public class ObligationDataAccess : IObligationDataAccess
    {
        private readonly IUserContext userContext;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public ObligationDataAccess(IUserContext userContext, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext)
        {
            this.userContext = userContext;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<List<int>> GetObligationComplianceYears(UKCompetentAuthority authority)
        {
            var obligationSchemes = weeeContext.ObligationSchemes.AsQueryable();

            if (authority != null)
            {
                obligationSchemes = obligationSchemes.Where(s => s.Scheme.CompetentAuthority.Id == authority.Id);
            }

            return await obligationSchemes
                .Select(s => s.ComplianceYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
        }

        public async Task<List<Scheme>> GetObligationSchemeData(UKCompetentAuthority authority, int complianceYear)
        {
            var schemes = weeeContext.Schemes.Where(s => s.SchemeStatus.Value == SchemeStatus.Approved.Value && s.CompetentAuthority.Id == authority.Id);

            return await schemes
                .IncludeFilter(s => s.ObligationSchemes.Where(s1 => s1.ComplianceYear == complianceYear))
                .ToListAsync();
        }

        public async Task<List<Scheme>> GetSchemesWithObligations(int complianceYear)
        {
            return await weeeContext.ObligationSchemes.Where(os => os.ComplianceYear == complianceYear)
                .Select(s => s.Scheme)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Guid> AddObligationUpload(UKCompetentAuthority ukCompetentAuthority,
            string data, 
            string fileName,
            IList<ObligationUploadError> errors,
            IList<ObligationScheme> obligations)
        {
            var obligationUpload = new ObligationUpload(ukCompetentAuthority, userContext.UserId.ToString(), data, fileName);
            var obligationsToAdd = new List<ObligationScheme>();

            if (!errors.Any())
            {
                foreach (var obligationScheme in obligations)
                {
                    // if there is no existing scheme obligation for the compliance year then it gets added
                    // otherwise the existing record is updated
                    var existingObligationScheme = obligationScheme.Scheme.ObligationSchemes.FirstOrDefault(o => o.ComplianceYear == obligationScheme.ComplianceYear);

                    if (obligationScheme.HasObligationSchemeAmount())
                    {
                        if (existingObligationScheme == null)
                        {
                            obligationsToAdd.Add(obligationScheme);
                        }
                        else
                        {
                            existingObligationScheme.UpdateObligationSchemeAmounts(obligationScheme.ObligationSchemeAmounts.ToList());
                        }
                    }

                    existingObligationScheme?.UpdateObligationUpload(obligationUpload);
                }

                if (obligationsToAdd.Any())
                {
                    obligationUpload.SetObligations(obligationsToAdd);
                }
            }
            else
            {
                obligationUpload.SetErrors(errors);
            }

            var updatedObligationUpload = await genericDataAccess.Add(obligationUpload);

            return updatedObligationUpload.Id;
        }
    }
}
