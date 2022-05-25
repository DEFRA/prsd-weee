namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using CsvHelper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, object>
    {
        public Task<object> HandleAsync(SubmitSchemeObligation message)
        {
            var data = new byte[1];

            using (var ms = new MemoryStream(data))
            {
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //var records = csv.GetRecords<Foo>();
                }
            }

            return null;
        }
    }
}
