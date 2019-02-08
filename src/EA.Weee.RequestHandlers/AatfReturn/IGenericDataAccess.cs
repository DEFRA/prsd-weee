namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;

    public interface IGenericDataAccess
    {
        Task<Guid> Add<TEntity>(TEntity entity) where TEntity : Entity;

        Task<TEntity> GetById<TEntity>(Guid id) where TEntity : Entity;

        Task<TEntity> GetById<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : Entity;
    }
}