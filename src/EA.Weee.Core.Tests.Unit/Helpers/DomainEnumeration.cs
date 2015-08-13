namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using Prsd.Core.Domain;

    public class DomainEnumeration : Enumeration
    {
        public static readonly DomainEnumeration Something = new DomainEnumeration(1, "Something");
        public static readonly DomainEnumeration SomethingElse = new DomainEnumeration(2, "Something Else");
        
        protected DomainEnumeration()
        {          
        }

        private DomainEnumeration(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
