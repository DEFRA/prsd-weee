namespace EA.Prsd.Core.Tests.Mvc
{
    using System.Web.Mvc;

    internal class FakeViewDataContainer : IViewDataContainer
    {
        private ViewDataDictionary viewData = new ViewDataDictionary();

        public ViewDataDictionary ViewData 
        {
            get 
            { 
                return viewData; 
            } 
            set 
            { 
                viewData = value; 
            } 
        }
    }
}
