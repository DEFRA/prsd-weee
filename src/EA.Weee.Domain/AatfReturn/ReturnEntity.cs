namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class ReturnEntity : Entity
    {
        public Return Return { get; private set; }

        public void UpdateReturn(Return @return)
        {
            Guard.ArgumentNotNull(() => @return, @return);

            Return = @return;
        }
    }
}
