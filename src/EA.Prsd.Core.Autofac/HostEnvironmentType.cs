namespace EA.Prsd.Core.Autofac
{
    using System.ComponentModel;

    public enum HostEnvironmentType
    {
        [Description("Unknown - this is a sign / cause of innevitable problems")]
        Unknown = 0,

        [Description("Web Api using IIS - can use PerWebRequest and HttpContext")]
        WebApi = 1,

        [Description("OWIN (Web Api) - can use PerWebRequest and OWIN context")]
        Owin = 2,

        [Description("Console app - e.g. the test runner or an importer")]
        Console = 3
    }
}
