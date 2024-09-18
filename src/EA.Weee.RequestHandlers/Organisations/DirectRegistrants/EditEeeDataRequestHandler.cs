namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.Helpers;
    using DataAccess;
    using Domain.Lookup;
    using Domain.Obligation;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Linq;
    using System.Threading.Tasks;

    internal class EditEeeDataRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<EditEeeDataRequest, bool>
    {
        private readonly WeeeContext weeeContext;

        public EditEeeDataRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(EditEeeDataRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

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