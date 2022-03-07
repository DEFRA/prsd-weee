namespace EA.Weee.Integration.Tests.Builders.Base
{
    using System;
    using Autofac;
    using Prsd.Core.Autofac;

    /// <summary>
    /// Use this if you want to create an instance and add things to it
    /// 
    /// If you want to defer the instantiation till the end, look at DeferredBuilder instead 
    ///     for example when setting up mocks to inject into a service
    /// </summary>
    /// <typeparam name="T">type of the instance you're building</typeparam>
    /// <typeparam name="K">type of the derived Builder</typeparam>
    public abstract class InstanceBuilder<T, K>
        where T : class
        where K : InstanceBuilder<T, K>
    {
        protected T instance = null;
        public IContainer Container; // only use this if you want to

        protected InstanceBuilder()
        {
            Container = ServiceLocator.Container;
        }

        protected abstract T Instantiate();

        /// <summary>
        /// This is a (simple case scenario) Init method that creates a type builder 
        /// for where there is no extra step e.g. save in db
        /// You can inject an action
        /// </summary>
        /// <param name="actions"></param>
        /// <returns>type of the instance you're building</returns>
        public static K Init(params Action<K>[] actions)
        {
            var builder = (K)Activator.CreateInstance(typeof(K));
            foreach (var action in actions)
            {
                action?.Invoke(builder);
            }
            builder.instance = builder.Instantiate();
            return builder;
        }

        public K With(Action<T> action)
        {
            action(instance);
            return (K)this;
        }

        /// <summary>
        /// This is a (simple case scenario) default create 
        /// for where there is no extra step e.g. save in db
        /// Override this as needed
        /// </summary>
        /// <returns>type of the instance you're building</returns>
        public virtual T Create()
        {
            if (instance == null)
            {
                throw new NotImplementedException(
                    "Please create and call methods in your derived class to build the initial instance with default values");
            }
                
            return instance;
        }
    }
}
