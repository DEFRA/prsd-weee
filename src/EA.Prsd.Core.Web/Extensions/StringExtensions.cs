namespace EA.Prsd.Core.Web.Extensions
{
    public static class StringExtensions
    {
        public static string EnsureTrailingSlash(this string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }
    }
}
