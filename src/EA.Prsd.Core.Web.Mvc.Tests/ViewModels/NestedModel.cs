namespace EA.Prsd.Core.Web.Mvc.Tests.ViewModels
{
    public class NestedModel
    {
        public string MiddleLevel { get; set; }

        public BottomModel Bottom { get; set; }
    }
}
