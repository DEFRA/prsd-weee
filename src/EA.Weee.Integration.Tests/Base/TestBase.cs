namespace EA.Weee.Integration.Tests.Base
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;

    public abstract class TestBase : IDisposable
    {
        public static Exception Exception;
        protected static StringBuilder log = new StringBuilder();

        public static void ShouldNotThrowException()
        {
            ShouldNotThrowException(null);
        }

        public static void ShouldNotThrowException(string because)
        {
            if (Exception != null)
            {
                //Console.WriteLine(Exception);
            }
                
            Exception.Should().BeNull(because);
        }

        public static void ShouldThrowException()
        {
            ShouldThrowException(null);
        }

        public static void ShouldThrowException(string because)
        {
            if (Exception != null)
            {
                //Console.WriteLine(Exception);
            }
                
            Exception.Should().NotBeNull(because);
        }

        public static void ShouldThrowException<T>() where T : Exception
        {
            ShouldThrowException<T>(null);
        }

        public static void ShouldThrowException<T>(string because) where T : Exception
        {
            if (Exception != null)
            {
                //Console.WriteLine(Exception);
            }
                
            Exception.Should().NotBeNull(because);
            Exception.Should().BeOfType<T>(because);
        }

        public static Exception CatchException(Action action)
        {
            Exception = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Exception = ex;
                //Console.WriteLine(ex);
            }

            return Exception;
        }

        public static void CatchExceptionAsync(Func<Task> action)
        {
            Exception = null;
            try
            {
                action().Wait();
            }
            catch (AggregateException ex)
            {
                Exception = ex.InnerExceptions.ElementAt(0);

                //Console.WriteLine(ex);
            }
        }

        public static void SetupLogger()
        {
        }

        public abstract void Dispose();
    }
}
