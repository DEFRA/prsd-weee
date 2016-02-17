namespace EA.Weee.Core.Logging
{
    using System;

    public interface ILogger
    {
        void Log(Exception exception);
    }
}
