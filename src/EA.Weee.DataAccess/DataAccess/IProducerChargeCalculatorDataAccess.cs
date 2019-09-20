namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System.Threading.Tasks;

    public interface IProducerChargeCalculatorDataAccess
    {
        Task<ChargeBandAmount> FetchCurrentChargeBandAmount(ChargeBand chargeBandType);
    }
}
