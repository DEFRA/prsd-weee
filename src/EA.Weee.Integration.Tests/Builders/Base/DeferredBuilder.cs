namespace EA.Weee.Integration.Tests.Builders.Base
{
    using System;
    using Autofac;
    using Prsd.Core.Autofac;

    /// <summary>
    /// Use this when you want to build something and only create the instance at the end (in Create())
    /// </summary>
    /// <typeparam name="T">type of the instance you're building</typeparam>
    /// <typeparam name="K">type of the derived Type Builder</typeparam>
    public abstract class DeferredBuilder<T, K>
        where T : class
        where K : DeferredBuilder<T, K>
    {
        public IContainer Container; // only use this if you want to

        protected DeferredBuilder()
        {
            Container = ServiceLocator.Container;
        }

        protected abstract void Instantiate();

        /// <summary>
        /// This is a (simple case scenario) Init method that creates a type builder 
        /// for where there is no extra step e.g. save in db
        /// You can inject an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns>type of the instance you're building</returns>
        public static K Init(Action<K> action = null)
        {
            var builder = (K)Activator.CreateInstance(typeof(K));
            action?.Invoke(builder);
            builder.Instantiate();
            return builder;
        }

        public abstract T Create();
    }
}
