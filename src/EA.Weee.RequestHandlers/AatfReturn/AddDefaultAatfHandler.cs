namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Charges;
    using Core.Shared;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Specification;

    public class AddDefaultAatfHandler : IRequestHandler<AddDefaultAatf, bool>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly WeeeContext context;

        public AddDefaultAatfHandler(IGenericDataAccess genericDataAccess, 
            ICommonDataAccess commonDataAccess, 
            WeeeContext context)
        {
            this.genericDataAccess = genericDataAccess;
            this.commonDataAccess = commonDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(AddDefaultAatf message)
        {
            var @operator = await genericDataAccess.GetById<Operator>(new OperatorByOrganisationIdSpecification(message.OrganisationId));
            
            if (@operator == null)
            {
                var organisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);

                @operator = new Operator(organisation);
            }

            var competantAuthority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

            if (!context.Aatfs.Any(a => a.Operator.Organisation.Id == message.OrganisationId))
            {
                context.Aatfs.AddRange(GetAatfs(@operator, competantAuthority));
            }

            await context.SaveChangesAsync();

            return true;
        }

        private List<Aatf> GetAatfs(Operator @operator, UKCompetentAuthority competentAuthority)
        {
            var aatfs = new List<Aatf>()
            {
                new Aatf("ABB Ltd Darlaston", competentAuthority, "123456789", AatfStatus.Approved, @operator),
                new Aatf("ABB Ltd Woking", competentAuthority, "123456789", AatfStatus.Approved, @operator),
                new Aatf("ABB Ltd Maidenhead", competentAuthority, "123456789", AatfStatus.Approved, @operator)
            };

            return aatfs;
        }
    }
}
