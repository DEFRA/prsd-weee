﻿namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Obligation;
    using Prsd.Core.Domain;
    using System;

    public class ReturnItem : Entity, IReturnItem, IEquatable<ReturnItem>
    {
        public virtual ObligationType ObligationType { get; private set; }

        public virtual decimal Tonnage { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public ReturnItem(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage)
        {
            WeeeCategory = weeeCategory;
            ObligationType = obligationType;

            if (tonnage < 0)
            {
                throw new ArgumentOutOfRangeException("amountInTonnes");
            }

            if (obligationType != ObligationType.B2B && obligationType != ObligationType.B2C)
            {
                string errorMessage = "The obligation type of a return item must be either B2B or B2C.";
                throw new InvalidOperationException(errorMessage);
            }

            Tonnage = tonnage;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected ReturnItem()
        {
        }

        /// <summary>
        /// This column should only be used by Entity Framework. it provides a mapping between the
        /// <see cref="ObligationType"/> enum and the NVARCHAR(4) stored in the database.
        /// </summary>
        public string DatabaseObligationType
        {
            get { return ObligationType.ToString(); }
            set { ObligationType = (ObligationType)Enum.Parse(typeof(ObligationType), value); }
        }

        public bool Equals(ReturnItem other)
        {
            if (other == null)
            {
                return false;
            }

            return ObligationType == other.ObligationType &&
                   WeeeCategory == other.WeeeCategory &&
                   Tonnage == other.Tonnage;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ReturnItem);
        }

        public override int GetHashCode()
        {
            return ObligationType.GetHashCode() ^ WeeeCategory.GetHashCode() ^ Tonnage.GetHashCode();
        }

        public void UpdateTonnage(decimal tonnage)
        {
            Tonnage = tonnage;
        }
    }
}
