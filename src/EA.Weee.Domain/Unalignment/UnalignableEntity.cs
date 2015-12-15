namespace EA.Weee.Domain.Unalignment
{
    using Prsd.Core.Domain;

    public abstract class UnalignableEntity : Entity
    {
        public bool IsAligned { get; private set; }

        protected UnalignableEntity()
        {
            IsAligned = true;
        }

        public virtual void Unalign()
        {
            IsAligned = false;
        }
    }
}
