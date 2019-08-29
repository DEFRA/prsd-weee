namespace EA.Prsd.Core.Mediator
{
    using System;

    /// <summary>
    ///     Represents void.
    /// </summary>
    public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        /// <summary>
        ///     Gets the single unit value.
        /// </summary>
        public static readonly Unit Value = new Unit();

        public int CompareTo(Unit other)
        {
            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public override string ToString()
        {
            return "()";
        }
    }
}