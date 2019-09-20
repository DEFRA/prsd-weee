namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using System;

    public class AddNonObligated : NonObligated
    {
        public bool Dcf { get; set; }

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
