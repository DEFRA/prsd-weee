namespace EA.Weee.DataAccess.DataAccess
{
    using System.Threading.Tasks;
    using Domain.Lookup;

    public interface IProducerChargeCalculatorDataAccess
    {
        Task<ChargeBandAmount> FetchCurrentChargeBandAmount(ChargeBand chargeBandType);
    }
}
