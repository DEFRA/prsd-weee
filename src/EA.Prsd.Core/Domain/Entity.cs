namespace EA.Prsd.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Extensions;

    public abstract class Entity : IComparable<Entity>
    {
        private readonly ICollection<IEvent> events = new List<IEvent>();

        public virtual Guid Id { get; private set; }

        [Browsable(false)]
        public byte[] RowVersion { get; set; }

        public IEnumerable<IEvent> Events
        {
            get { return events.ToSafeIEnumerable(); }
        }

        public int CompareTo(Entity other)
        {
            return IsPersistent() ? Id.CompareTo(other.Id) : 0;
        }

        public void RaiseEvent(IEvent @event)
        {
            events.Add(@event);
        }

        public void ClearEvents()
        {
            events.Clear();
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