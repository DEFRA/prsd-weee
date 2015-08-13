namespace EA.Weee.Domain
{
    using System;

    public class Country : IEquatable<Country>
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

        public virtual bool Equals(Country other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Country);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
