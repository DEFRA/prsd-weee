namespace EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using Weee.DataAccess;

    public class FindMatchingOrganisationsDataAccess : IFindMatchingOrganisationsDataAccess
    {
        private readonly WeeeContext context;

        public FindMatchingOrganisationsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Organisation[]> GetOrganisationsBySimpleSearchTerm(string searchTerm, Guid userId)
        {
            var firstLetterOfSearchTerm = searchTerm[0].ToString();

            var organisationsCurrentlyLinkedToUser = await OrganisationsCurrentlyLinkedToUser(userId);

            return (await context.Organisations.ToListAsync())
                .Where(o =>
                {
                    var organisationNameStartsWithThe = (!string.IsNullOrEmpty(o.TradingName) &&
                                                         o.TradingName.StartsWith("THE ",
                                                             StringComparison.InvariantCultureIgnoreCase))
                                                        ||
                                                        (!string.IsNullOrEmpty(o.Name) &&
                                                         o.Name.StartsWith("THE ",
                                                             StringComparison.InvariantCultureIgnoreCase));

                    var organisationNameStartsWithFirstLetter = (!string.IsNullOrEmpty(o.TradingName) &&
                                                                 o.TradingName.StartsWith(firstLetterOfSearchTerm,
                                                                     StringComparison.InvariantCultureIgnoreCase))
                                                                ||
                                                                (!string.IsNullOrEmpty(o.Name) &&
                                                                 o.Name.StartsWith(firstLetterOfSearchTerm,
                                                                     StringComparison.InvariantCultureIgnoreCase));

                    var organisationIsComplete = o.OrganisationStatus == OrganisationStatus.Complete;

                    var hasOutstandingJoinRequest = organisationsCurrentlyLinkedToUser.Any(id => id == o.Id);

                    return organisationIsComplete
                           && (organisationNameStartsWithThe || organisationNameStartsWithFirstLetter)
                           && !hasOutstandingJoinRequest;
                }).ToArray();
        }

        private async Task<IEnumerable<Guid>> OrganisationsCurrentlyLinkedToUser(Guid userId)
        {
            return (await context.OrganisationUsers
                .Where(ou => ou.UserId == userId.ToString()
                             && ou.UserStatus.Value != UserStatus.Rejected.Value)
                .ToArrayAsync())
                .Select(ou => ou.OrganisationId);
        }
    }
}
