namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    /// <summary>
    /// This request returns a list of schemes.
    /// The list will be filtered to only include schemes with specified statuses.
    /// The results will be returned ordered by scheme name from A to Z.
    /// </summary>
    public class GetSchemes : IRequest<List<SchemeData>>
    {
        /// <summary>
        /// Deteremines the filter that will be applied when fetching the list of schemes.
        /// </summary>
        public enum FilterType
        {
            /// <summary>
            /// Only schemes with a scheme status of "Approved" will be included.
            /// </summary>
            Approved,

            /// <summary>
            /// Only schemes with a scheme status of "Approved" or "Withdrawn" will be included.
            /// </summary>
            ApprovedOrWithdrawn
        }

        public FilterType Filter { get; private set; }

        public GetSchemes(FilterType filter)
        {
            Filter = filter;
        }
    }
}
