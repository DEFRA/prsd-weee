namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetSmallProducerDirectRegistrantChargeRequestHandler : IRequestHandler<GetSmallProducerDirectRegistrantChargeRequest, SmallProducerDirectRegistrantChargeData>
    {
        private readonly WeeeContext context;

        public GetSmallProducerDirectRegistrantChargeRequestHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<SmallProducerDirectRegistrantChargeData> HandleAsync(GetSmallProducerDirectRegistrantChargeRequest request)
        {
            var smallProducerDirectRegistrantCharge = await context.DirectRegistrantCharges.Where(d => d.ComplianceYear == request.ComplianceYear)
                                                                                           .FirstOrDefaultAsync();

            return new SmallProducerDirectRegistrantChargeData()
            {
                ChargeAmount = smallProducerDirectRegistrantCharge.ChargeAmount,
                ComplianceYear = smallProducerDirectRegistrantCharge.ComplianceYear,
                EffectiveFrom = smallProducerDirectRegistrantCharge.EffectiveFrom,
                Id = smallProducerDirectRegistrantCharge.Id
            };
        }
    }
}
