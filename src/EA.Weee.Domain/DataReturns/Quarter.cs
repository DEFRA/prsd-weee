﻿namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Diagnostics;

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

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected Quarter()
        {
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

        public string ConvertToString(Quarter quarter)
        {
            var q = quarter.Q.ToString();
            var period = string.Empty;
            switch (q)
            {
                case "Q1":
                    period = "Jan - Mar";
                    break;
                case "Q2":
                    period = "Apr - Jun";
                    break;
                case "Q3":
                    period = "Jul - Sep";
                    break;
                case "Q4":
                    period = "Oct - Dec";
                    break;
            }
            var stringHolder = q + " " + period;
            return stringHolder;
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
