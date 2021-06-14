namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn.NonObligated;
    using Security;

    internal class AddNonObligatedHandler : IRequestHandler<AddNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IReturnDataAccess returnDataAccess;

        public AddNonObligatedHandler(IWeeeAuthorization authorization,
            INonObligatedDataAccess nonObligatedDataAccess,
            IReturnDataAccess returnDataAccess)
        {
            this.authorization = authorization;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.returnDataAccess = returnDataAccess;
        }

        public async Task<bool> HandleAsync(AddNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfReturn = await returnDataAccess.GetById(message.ReturnId);

            // Check that the data we are adding does not have any duplicate categories in
            var nonObligatedWee = message.CategoryValues
                .GroupBy(n => n.CategoryId)
                .Select(g => g.FirstOrDefault())
                .Where(n => n != null)
                .Select(n => new NonObligatedWeee(aatfReturn, n.CategoryId, message.Dcf, n.Tonnage));

            // Ensure we are not adding additional non-obligated WEEE, if there are existing, delete them
            var existingToDelete = (await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId))
                .GroupBy(n => n.CategoryId, (cat, col) => new { cat, col })
                .Where(g => nonObligatedWee.Any(f => f.CategoryId == g.cat))
                .SelectMany(g => g.col);

            if (existingToDelete.Count() > 0)
            {
                await nonObligatedDataAccess.Remove(existingToDelete);
            }

            // Add the non-Obligated WEEE that should be there
            await nonObligatedDataAccess.Submit(nonObligatedWee);

            return true;
        }
    }
}
