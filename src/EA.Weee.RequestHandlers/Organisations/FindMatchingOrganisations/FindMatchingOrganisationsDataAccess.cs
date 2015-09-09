namespace EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Domain;

    public class FindMatchingOrganisationsDataAccess : IFindMatchingOrganisationsDataAccess
    {
        private readonly WeeeContext context;
        private readonly IUserContext userContext;

        public FindMatchingOrganisationsDataAccess(WeeeContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Organisation[]> GetOrganisationsBySimpleSearchTerm(string searchTerm)
        {
            var firstLetterOfSearchTerm = searchTerm[0].ToString();

            return await context.Organisations
                .Where(o => (OrganisationNameStartsWithThe(o) || OrganisationNameStartsWithFirstLetter(o, firstLetterOfSearchTerm))
                            && o.OrganisationStatus == OrganisationStatus.Complete
                            && IsAbleToJoinOrganisation(o.Id))
                .ToArrayAsync();
        }

        private bool OrganisationNameStartsWithThe(Organisation o)
        {
            return (!string.IsNullOrEmpty(o.TradingName) && o.TradingName.StartsWith("THE ", StringComparison.InvariantCultureIgnoreCase))
                   || (!string.IsNullOrEmpty(o.Name) && o.Name.StartsWith("THE ", StringComparison.InvariantCultureIgnoreCase));
        }

        private bool OrganisationNameStartsWithFirstLetter(Organisation o, string firstLetter)
        {
            return (!string.IsNullOrEmpty(o.TradingName) && o.TradingName.StartsWith(firstLetter, StringComparison.InvariantCultureIgnoreCase))
                   || (!string.IsNullOrEmpty(o.Name) && o.Name.StartsWith(firstLetter, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IsAbleToJoinOrganisation(Guid organisationId)
        {
            return !context.OrganisationUsers.Any(o => o.OrganisationId == organisationId
                                                       && o.UserId == userContext.UserId.ToString()
                                                       && o.UserStatus.Value != UserStatus.Rejected.Value);
        }
    }
}
