namespace EA.Weee.Sroc.Migration
{
    public interface IUpdateProducerCharges
    {
        void UpdateCharges();

        void RollbackCharges();
    }
}