namespace EA.Weee.Requests.Organisations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContactData
    {
        public Guid Id { get; set; }
    
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Position { get; set; }
    }
}
