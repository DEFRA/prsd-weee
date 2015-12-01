namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [DebuggerDisplay("{Year} {Q}")]
    public class Quarter : IComparable<Quarter>, IEquatable<Quarter>
    {
        public int Year { get; private set; }

        public QuarterType Q { get; private set; }

        public Quarter(int year, QuarterType q)
        {
            if (year < 1900 || year >= 3000)
            {
                throw new ArgumentOutOfRangeException("year");
            }

            Year = year;
            Q = q;
        }

        public bool Equals(Quarter other)
        {
            if (other == null)
            {
                return false;
            }

            return Year == other.Year
                && Q == other.Q;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Quarter quarterObj = obj as Quarter;
            if (quarterObj == null)
            {
                return false;
            }
            else
            {
                return Equals(quarterObj);
            }
        }

        public override int GetHashCode()
        {
            return ToInteger().GetHashCode();
        }

        public int CompareTo(Quarter other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            int thisInteger = this.ToInteger();
            int otherInteger = other.ToInteger();
            return thisInteger.CompareTo(otherInteger);
        }

        public static bool operator ==(Quarter x, Quarter y)
        {
            if (((object)x) == null || ((object)y) == null)
            {
                return Object.Equals(x, y);
            }
            else
            {
                return x.Equals(y);
            }
        }

        public static bool operator !=(Quarter x, Quarter y)
        {
            if (((object)x) == null || ((object)y) == null)
            {
                return !Object.Equals(x, y);
            }
            else
            {
                return !x.Equals(y);
            }
        }

        public static bool operator >(Quarter x, Quarter y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.CompareTo(y) > 0;
            }
        }

        public static bool operator <(Quarter x, Quarter y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.CompareTo(y) < 0;
            }
        }

        public static bool operator >=(Quarter x, Quarter y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.CompareTo(y) >= 0;
            }
        }

        public static bool operator <=(Quarter x, Quarter y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.CompareTo(y) <= 0;
            }
        }

        private int ToInteger()
        {
            return (Year * 4) + (int)Q;
        }
    }
}
