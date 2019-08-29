namespace EA.Prsd.Core.Tests.Stubs
{
    using System.Web;

    public class StubbedHttpPostedFile : HttpPostedFileBase
    {
        private readonly string _contentType;

        public override string ContentType
        {
            get { return _contentType; }
        }

        public StubbedHttpPostedFile(string contentType)
        {
            _contentType = contentType;
        }
    }
}
