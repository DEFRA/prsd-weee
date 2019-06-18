namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using DataReturns;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using Organisation;
    using User;

    public partial class Return : Entity
    {
        protected Return()
        {
        }

        public Return(Organisation organisation, Quarter quarter, string createdBy, FacilityType facilityType)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);
            Guard.ArgumentNotNull(() => facilityType, facilityType);

            Organisation = organisation;
            Quarter = quarter;
            ReturnStatus = ReturnStatus.Created;
            CreatedById = createdBy;
            CreatedDate = SystemTime.UtcNow;
            FacilityType = facilityType;
        }

        public virtual void UpdateSubmitted(string submittedBy, bool nilReturn)
        {
            Guard.ArgumentNotNullOrEmpty(() => submittedBy, submittedBy);

            if (ReturnStatus != ReturnStatus.Created)
            {
                throw new InvalidOperationException("Return status must be Created to transition to Submitted");
            }

            SubmittedById = submittedBy;
            SubmittedDate = SystemTime.UtcNow;
            ReturnStatus = ReturnStatus.Submitted;
            NilReturn = nilReturn;
        }

        public virtual void ResetSubmitted(string createdBy, Guid parentId)
        {
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);

            if (ReturnStatus != ReturnStatus.Submitted)
            {
                throw new InvalidOperationException("Return status must be Submitted if being copied");
            }

            SubmittedById = null;
            SubmittedBy = null;
            SubmittedDate = null;
            CreatedBy = null;
            CreatedById = createdBy;
            CreatedDate = SystemTime.UtcNow;
            ReturnStatus = ReturnStatus.Created;
            ParentId = parentId;
            NilReturn = false;
        }

        public virtual Quarter Quarter { get; private set; }

        public virtual ReturnStatus ReturnStatus { get; set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual DateTime CreatedDate { get; private set; }

        public virtual DateTime? SubmittedDate { get; set; }

        public virtual string CreatedById { get; private set; }
        
        public virtual string SubmittedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual User SubmittedBy { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual FacilityType FacilityType { get; set; }

        public virtual bool NilReturn { get; set; }
    }
}