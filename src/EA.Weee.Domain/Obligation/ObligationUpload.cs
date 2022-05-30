﻿namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
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

        [ForeignKey("CompetentAuthorityId")]
        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual Guid CompetentAuthorityId { get; private set; }

        public virtual ICollection<ObligationUploadError> ObligationUploadErrors { get; protected set; }

        public ObligationUpload()
        {
        }

        public ObligationUpload(UKCompetentAuthority competentAuthority,
            string uploadedById,
            string data,
            string fileName,
            ICollection<ObligationUploadError> errors)
        {
            Condition.Requires(data).IsNotNullOrWhiteSpace();
            Condition.Requires(fileName).IsNotNullOrWhiteSpace();
            Condition.Requires(uploadedById).IsNotNullOrWhiteSpace();
            Condition.Requires(competentAuthority).IsNotNull();

            CompetentAuthority = competentAuthority;
            Data = data;
            FileName = fileName;
            UploadedById = uploadedById;
            UploadedDate = SystemTime.UtcNow;
            ObligationUploadErrors = errors;
        }
    }
}
