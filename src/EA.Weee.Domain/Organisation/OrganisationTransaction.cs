namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using System;

    public partial class OrganisationTransaction : Entity
    {
        public OrganisationTransaction(string userId)
        {
            UserId = userId;
            CompletionStatus = CompletionStatus.Incomplete;
            CreatedDateTime = SystemTime.UtcNow;
            OrganisationJson = string.Empty;
        }

        public OrganisationTransaction()
        {
        }

        public string UserId { get; private set; }

        public virtual User.User User { get; private set; }

        public virtual CompletionStatus CompletionStatus { get; set; }

        public virtual string OrganisationJson { get; set; }

        public virtual DateTime CreatedDateTime { get; private set; }

        public virtual DateTime? CompletedDateTime { get; private set; }

        public virtual Guid? OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public void CompleteRegistration()
        {
            ToComplete();
        }

        private void ToComplete()
        {
            if (CompletionStatus != CompletionStatus.Incomplete)
            {
                throw new InvalidOperationException("Completion status must be Incomplete to transition to Complete");
            }
            CompletionStatus = CompletionStatus.Complete;
            CompletedDateTime = SystemTime.UtcNow;
        }

        public void SetOrganisationJson(string json)
        {
            OrganisationJson = json;
        }
    }
}