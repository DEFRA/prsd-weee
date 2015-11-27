namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;
    using System;

    public class SICCode : Entity, IEquatable<SICCode>, IComparable<SICCode>
    {
        public SICCode(string name)
        {
            Name = name;
        }

        protected SICCode()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(SICCode other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SICCode);
        }

        public int CompareTo(SICCode other)
        {
            if (other == null)
            {
                return 1;
            }

            return Name.CompareTo(other.Name);
        }

        public string Name { get; private set; }

        public virtual Guid ProducerId { get; private set; }

        public virtual Producer Producer { get; private set; }
    }
}
