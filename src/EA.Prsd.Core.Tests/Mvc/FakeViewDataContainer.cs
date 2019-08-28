namespace EA.Prsd.Core.Tests.Mvc
{
    using System.Web.Mvc;

    internal class FakeViewDataContainer : IViewDataContainer
    {
        private ViewDataDictionary _viewData = new ViewDataDictionary();
        public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
    }
}
