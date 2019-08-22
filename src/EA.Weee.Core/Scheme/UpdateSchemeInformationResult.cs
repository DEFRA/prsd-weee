﻿namespace EA.Weee.Core.Scheme
{
    /// <summary>
    /// This DTO provides the result of a request to update scheme information.
    /// </summary>
    public class UpdateSchemeInformationResult
    {
        /// <summary>
        /// Indicates whether the operation was successful or the nature of the failure.
        /// </summary>
        public ResultType Result { get; set; }

        /// <summary>
        /// Where the result of the operation indicates an failure of the 1B1S customer
        /// reference uniqueness constraint, this property will be populated with the
        /// deatils of the other scheme that already has the 1B1S customer reference.
        /// </summary>
        public IbisCustomerReferenceUniquenessFailureInfo IbisCustomerReferenceUniquenessFailure { get; set; }

        public enum ResultType
        {
            /// <summary>
            /// The operation was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// The operation failed because the specified approval number is already
            /// in use by another scheme.
            /// </summary>
            ApprovalNumberUniquenessFailure = 1,

            /// <summary>
            /// The operation failed because the specified 1B1S customer reference is already
            /// in use by another scheme.
            /// </summary>
            IbisCustomerReferenceUniquenessFailure = 2,

            /// <summary>
            /// The operation failed because the 1B1S customer reference was being changed
            /// to a blank/null value, which is invalid for schemes within the Environment Agency.
            /// </summary>
            IbisCustomerReferenceMandatoryForEAFailure = 3,
        }

        public class IbisCustomerReferenceUniquenessFailureInfo
        {
            public string IbisCustomerReference { get; set; }

            public string OtherSchemeName { get; set; }

            public string OtherSchemeApprovalNumber { get; set; }
        }
    }
}
