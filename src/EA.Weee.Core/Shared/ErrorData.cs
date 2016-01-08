namespace EA.Weee.Core.Shared
{
    using System;
    using Prsd.Core;
    public class ErrorData : IEquatable<ErrorData>
    {
        public ErrorData(string description, ErrorLevel errorLevel)
        {
            Guard.ArgumentNotNullOrEmpty(() => description, description);

            ErrorLevel = errorLevel;
            Description = description;
        }

        public ErrorLevel ErrorLevel { get; private set; }

        public string Description { get; private set; }

        public bool Equals(ErrorData other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            //Check whether the ErrorData's properties are equal.
            return Description.Equals(other.Description) && ErrorLevel.Equals(other.ErrorLevel);
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode() ^ ErrorLevel.GetHashCode();
        }
    }
}