namespace EA.Prsd.Core.Domain
{
    using System;
    using System.ComponentModel;

    public abstract class Entity : IComparable<Entity>
    {
        public Guid Id { get; private set; }

        [Browsable(false)]
        public byte[] RowVersion { get; set; }

        public int CompareTo(Entity other)
        {
            return IsPersistent() ? Id.CompareTo(other.Id) : 0;
        }

        public override bool Equals(object obj)
        {
            if (IsPersistent())
            {
                var entity = obj as Entity;
                return (entity != null) && (Id == entity.Id);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return IsPersistent() ? Id.GetHashCode() : base.GetHashCode();
        }

        private bool IsPersistent()
        {
            return (Id != Guid.Empty);
        }
    }
}