namespace EA.Prsd.Core.Tests.Mvc
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

    internal class FakeHttpContext : HttpContextBase
    {
        private readonly Dictionary<object, object> fakeItems = new Dictionary<object, object>();

        public override IDictionary Items 
        { 
            get 
            { 
                return fakeItems; 
            } 
        }
    }
}
