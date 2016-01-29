namespace EA.Weee.Domain.Security
{
    using Prsd.Core.Domain;

    public class Role : Entity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        protected Role()
        {
        }
    }
}
