namespace EA.Prsd.Core.Security
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class AllowUnauthorizedUserAttribute : Attribute
    {
    }
}