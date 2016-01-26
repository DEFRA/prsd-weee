namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Lookup;
    using Obligation;

    public class WeeeCollectedAmount : ReturnItem, IEquatable<WeeeCollectedAmount>
    {
        public virtual WeeeCollectedAmountSourceType SourceType { get; private set; }

        public virtual ICollection<WeeeCollectedReturnVersion> WeeeCollectedReturnVersions { get; private set; }

        public WeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage) :
            base(obligationType, weeeCategory, tonnage)
        {
            SourceType = sourceType;
            WeeeCollectedReturnVersions = new List<WeeeCollectedReturnVersion>();
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected WeeeCollectedAmount()
        {
        }

        public bool Equals(WeeeCollectedAmount other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals((ReturnItem)other) &&
                   SourceType == other.SourceType;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ SourceType.GetHashCode();
        }
    }
}
