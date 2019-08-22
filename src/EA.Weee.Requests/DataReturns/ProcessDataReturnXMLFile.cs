namespace EA.Weee.Requests.DataReturns
{
    using Prsd.Core.Mediator;
    using System;

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