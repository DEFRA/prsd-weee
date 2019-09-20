namespace EA.Weee.Web
{
    using Autofac;
    using FluentValidation;
    using System;

    public class AutofacValidatorFactory : IValidatorFactory
    {
        private readonly IComponentContext container;

        public AutofacValidatorFactory(IComponentContext container)
        {
            this.container = container;
        }

        public IValidator<T> GetValidator<T>()
        {
            return (IValidator<T>)GetValidator(typeof(T));
        }

        public IValidator GetValidator(Type type)
        {
            var genericType = typeof(IValidator<>).MakeGenericType(type);
            if (container.TryResolve(genericType, out var validator))
            {
                return (IValidator)validator;
            }

            return null;
        }
    }
}