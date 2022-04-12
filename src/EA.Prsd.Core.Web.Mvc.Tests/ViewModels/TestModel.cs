namespace EA.Prsd.Core.Web.Mvc.Tests.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class TestModel
    {
        public string TopLevel { get; set; }

        public NestedModel Nested { get; set; }

        [Required]
        public string RequiredField { get; set; }

        public DateTime Date { get; set; }
    }
}
