namespace EA.Weee.Web.Requests.Base
{
    using AutoMapper;

    public abstract class RequestCreator<TViewModel, TRequest> : IRequestCreator<TViewModel, TRequest>
    {
        /// <summary>
        /// Provides a default mapping from a view model to a request using AutoMapper. 
        /// This mapper should not be altered but inherited from and overriden where the mapping is more complex
        /// </summary>
        /// <param name="viewModel">The view model to be mapped</param>
        /// <returns>The resulting request created from this view model</returns>
        public virtual TRequest ViewModelToRequest(TViewModel viewModel)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<TViewModel, TRequest>();
            });

            return Mapper.Map<TViewModel, TRequest>(viewModel);
        }
    }
}