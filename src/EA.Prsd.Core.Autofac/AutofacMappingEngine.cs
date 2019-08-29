namespace EA.Prsd.Core.Autofac
{
    using global::Autofac;
    using Mapper;

    public class AutofacMappingEngine : IMapper
    {
        private readonly IComponentContext context;
        private readonly ITypeResolver typeResolver;

        public AutofacMappingEngine(IComponentContext context, ITypeResolver typeResolver)
        {
            this.context = context;
            this.typeResolver = typeResolver;
        }

        public AutofacMappingEngine(IComponentContext context)
        {
            this.context = context;
            typeResolver = new DefaultTypeResolver();
        }

        public TResult Map<TSource, TResult>(TSource source)
        {
            if (source == null)
            {
                return default(TResult);
            }

            return context.Resolve<IMap<TSource, TResult>>().Map(source);
        }

        public TResult Map<TResult>(object source)
        {
            if (source == null)
            {
                return default(TResult);
            }

            var sourceType = typeResolver.GetType(source);
            var mapper = context.Resolve(typeof(IMap<,>).MakeGenericType(sourceType, typeof(TResult)));
            return (TResult)mapper.GetType().GetMethod("Map", new[] { sourceType }).Invoke(mapper, new[] { source });
        }

        public TResult Map<TSource, TParameter, TResult>(TSource source, TParameter parameter)
        {
            if (source == null)
            {
                return default(TResult);
            }

            return context.Resolve<IMapWithParameter<TSource, TParameter, TResult>>().Map(source, parameter);
        }

        public TResult Map<TResult>(object source, object parameter)
        {
            if (source == null)
            {
                return default(TResult);
            }

            var sourceType = typeResolver.GetType(source);
            var parameterType = typeResolver.GetType(parameter);
            var mapper =
                context.Resolve(typeof(IMapWithParameter<,,>).MakeGenericType(sourceType, parameterType,
                    typeof(TResult)));
            return
                (TResult)
                    mapper.GetType()
                        .GetMethod("Map", new[] { sourceType, parameterType })
                        .Invoke(mapper, new[] { source, parameter });
        }
    }
}