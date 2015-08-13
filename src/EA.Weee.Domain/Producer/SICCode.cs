﻿namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class SICCode : Entity, IEquatable<SICCode>
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

        public string Name { get; private set; }

        public virtual Guid ProducerId { get; private set; }

        public virtual Producer Producer { get; private set; }
    }
}
