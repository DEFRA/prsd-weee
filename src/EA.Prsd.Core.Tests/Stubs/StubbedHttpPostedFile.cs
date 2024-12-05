namespace EA.Prsd.Core.Tests.Stubs
{
    using System.Web;

    public class StubbedHttpPostedFile : HttpPostedFileBase
    {
        private readonly string contentType;

        public override string ContentType => contentType;

        public StubbedHttpPostedFile(string contentType)
        {
            this.contentType = contentType;
        }
    }
}
