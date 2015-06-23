namespace EA.Weee.Domain
{
    using System;

    public class Country
    {
        public Country(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        protected Country()
        {
        }

        public Guid Id { get; protected set; }

        public string Name { get; private set; }
    }
}
