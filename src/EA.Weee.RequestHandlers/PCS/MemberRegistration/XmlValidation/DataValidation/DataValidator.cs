namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.DataValidation
{
    using System;
    using System.Collections.Generic;
    using DataAccess;
    using Domain.PCS;

    public class DataValidator : IDataValidator
    {
        private readonly WeeeContext context;

        public DataValidator(WeeeContext context)
        {
            this.context = context;
        }

        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml)
        {
            // TODO: Add validation against existing data here:

            return new List<MemberUploadError>();
        }
    }
}
