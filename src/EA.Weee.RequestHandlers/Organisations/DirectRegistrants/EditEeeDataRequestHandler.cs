using EA.Weee.Core.Helpers;

namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Linq;
    using System.Threading.Tasks;

    internal class EditEeeDataRequestHandler : IRequestHandler<EditEeeDataRequest, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;

        public EditEeeDataRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(EditEeeDataRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);
            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);
            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.First(r => r.ComplianceYear == SystemTime.UtcNow.Year);

            currentYearSubmission.CurrentSubmission.SellingTechniqueType = request.SellingTechniqueType.ToInt();

            if (currentYearSubmission.CurrentSubmission.EeeOutputReturnVersion == null)
            {
                var eeeVersion = new EeeOutputReturnVersion();
                foreach (var eee in request.TonnageData)
                {
                    eeeVersion.EeeOutputAmounts.Add(new EeeOutputAmount((ObligationType)eee.ObligationType, (WeeeCategory)eee.Category, eee.Tonnage, currentYearSubmission.RegisteredProducer));
                }
                currentYearSubmission.CurrentSubmission.EeeOutputReturnVersion = eeeVersion;
            }
            else
            {
                var eeeVersion = currentYearSubmission.CurrentSubmission.EeeOutputReturnVersion;
                foreach (var eee in request.TonnageData)
                {
                    var existingAmount = eeeVersion.EeeOutputAmounts
                        .FirstOrDefault(e => e.ObligationType == (ObligationType)eee.ObligationType
                                             && e.WeeeCategory == (WeeeCategory)eee.Category);
                    if (existingAmount != null)
                    {
                        existingAmount.UpdateTonnage(eee.Tonnage);
                    }
                    else
                    {
                        eeeVersion.EeeOutputAmounts.Add(new EeeOutputAmount(
                            (ObligationType)eee.ObligationType,
                            (WeeeCategory)eee.Category,
                            eee.Tonnage,
                            currentYearSubmission.RegisteredProducer));
                    }
                }

                // Remove entries that no longer exist in the request
                var amountsToKeep = eeeVersion.EeeOutputAmounts.Where(existing =>
                    request.TonnageData.Any(requested =>
                        (ObligationType)requested.ObligationType == existing.ObligationType
                        && (WeeeCategory)requested.Category == existing.WeeeCategory)).ToList();

                eeeVersion.EeeOutputAmounts.Clear();
                foreach (var amount in amountsToKeep)
                {
                    eeeVersion.EeeOutputAmounts.Add(amount);
                }
            }

            await weeeContext.SaveChangesAsync();
            return true;
        }
    }
}