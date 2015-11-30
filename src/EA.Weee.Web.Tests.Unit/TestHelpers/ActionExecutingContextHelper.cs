namespace EA.Weee.Web.Tests.Unit.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using FakeItEasy;

    public class ActionExecutingContextHelper
    {
        public static ActionDescriptor FakeActionDescriptorWithActionName(string actionName)
        {
            var fakeActionDescriptor = A.Fake<ActionDescriptor>();
            A.CallTo(() => fakeActionDescriptor.ActionName).Returns(actionName);

            return fakeActionDescriptor;
        }

        public static IDictionary<string, object> FakeActionParameters()
        {
            return A.Fake<IDictionary<string, object>>();
        }

        public static IDictionary<string, object> FakeActionParameters(bool retrievalResult, Guid outValue)
        {
            var fakeActionParameters = FakeActionParameters();

            object dummyObject;
            A.CallTo(() => fakeActionParameters.TryGetValue(A<string>._, out dummyObject))
                .Returns(retrievalResult)
                .AssignsOutAndRefParameters(outValue);

            return fakeActionParameters;
        }
    }
}
