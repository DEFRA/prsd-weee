namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.DataReturns;
    using Prsd.Core;

    public class CreateTestXmlFileHandler : IRequestHandler<CreateTestXmlFile, FileInfo>
    {
        public async Task<FileInfo> HandleAsync(Requests.DataReturns.CreateTestXmlFile message)
        {
            string fileName = string.Format("PCS Data Return XML - {0:yyyy MM dd HH mm ss}.xml", SystemTime.UtcNow);

            // TODO: Use the specified settings to generate XML.

            FileInfo file = new FileInfo(fileName, new byte[1]);

            return await Task.FromResult(file);
        }
    }
}
