namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using User;

    public partial class ObligationUpload : Entity
    {
        public virtual DateTime UploadedDate { get; private set; }

        public virtual string UploadedById { get; private set; }

        public virtual string Data { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual User UploadedBy { get; set; }

        public virtual ICollection<ObligationUploadError> ObligationUploadErrors { get; protected set; }

        public ObligationUpload(string uploadedById,
            string data,
            string fileName)
        {
            Condition.Requires(data).IsNotNullOrWhiteSpace();
            Condition.Requires(fileName).IsNotNullOrWhiteSpace();
            Condition.Requires(uploadedById).IsNotNullOrWhiteSpace();

            Data = data;
            FileName = fileName;
            UploadedById = uploadedById;
            UploadedDate = SystemTime.UtcNow;
            ObligationUploadErrors = new List<ObligationUploadError>();
        }
    }
}
