﻿namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class Partner : Entity, IEquatable<Partner>
    {
        public Partner(string name)
        {
            Name = name;
        }

        protected Partner()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Partner other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Partner);
        }

        public string Name { get; private set; }

        public virtual Guid PartnershipId { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}
