namespace EA.Prsd.Core.Tests.Stubs
{
    using System.Web;

    public class StubbedHttpPostedFile : HttpPostedFileBase
    {
        private readonly string contentType;

        public override string ContentType
        {
            get { return contentType; }
        }

        public StubbedHttpPostedFile(string contentType)
        {
            this.contentType = contentType;
        }
    }
}
