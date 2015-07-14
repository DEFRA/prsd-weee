namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class Partner : Entity
    {
        public Partner(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
