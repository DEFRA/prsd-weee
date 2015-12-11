namespace EA.Weee.Requests.DataReturns
{
    using System;
    using Prsd.Core.Mediator;

    public class ProcessDataReturnXmlFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public byte[] Data { get; private set; }

        public string FileName { get; private set; }

        public ProcessDataReturnXmlFile(Guid organisationId, byte[] data, string fileName)
        {
            OrganisationId = organisationId;
            Data = data;
            FileName = fileName;
        }
    }
}