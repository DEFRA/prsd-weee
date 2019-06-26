namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class ReturnAatf : Entity
    {
        public virtual Aatf Aatf { get; private set; }

        public virtual Return Return { get; private set; }

        public ReturnAatf(Aatf aatf, Return @return)
        {
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => @return, @return);

            Aatf = aatf;
            Return = @return;
        }
    }
}
