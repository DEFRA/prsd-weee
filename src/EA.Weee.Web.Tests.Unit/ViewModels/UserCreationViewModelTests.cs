namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using EA.Prsd.Core.Validation;
    using EA.Weee.Web.ViewModels.NewUser;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class UserCreationViewModelTests
    {
        [Fact]
        public void NewUser_MustBeTrueAttributeForPrivacy_MustBePresent()
        {
            Type t = typeof(UserCreationViewModel);
            PropertyInfo pi = t.GetProperty("PrivacyPolicy");
            Assert.True(Attribute.IsDefined(pi, typeof(MustBeTrueAttribute)));

            var attr = (MustBeTrueAttribute[])pi.GetCustomAttributes(typeof(MustBeTrueAttribute), false);

            Assert.True(attr[0].ErrorMessage == "Confirm that you've read and accepted the privacy policy");
        }

        [Fact]
        public void NewUser_MustBeTrueAttributeForTermsAndConditions_MustBePresent()
        {
            Type t = typeof(UserCreationViewModel);
            PropertyInfo pi = t.GetProperty("TermsAndConditions");
            Assert.True(Attribute.IsDefined(pi, typeof(MustBeTrueAttribute)));

            var attr = (MustBeTrueAttribute[])pi.GetCustomAttributes(typeof(MustBeTrueAttribute), false);

            Assert.True(attr[0].ErrorMessage == "Confirm that you've read and accepted the terms and conditions");
        }
    }
}
