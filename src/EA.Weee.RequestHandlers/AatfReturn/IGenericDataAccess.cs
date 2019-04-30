namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;
    using Specification;

    public interface IGenericDataAccess
    {
        Task<Guid> Add<TEntity>(TEntity entity) where TEntity : Entity;

        Task AddMany<TEntity>(IEnumerable<TEntity> entity) where TEntity : Entity;

        Task<List<TEntity>> GetAll<TEntity>() where TEntity : class;

        Task<TEntity> GetById<TEntity>(Guid id) where TEntity : Entity;

        Task<TEntity> GetSingleByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity;

        Task<List<TEntity>> GetManyByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity;
    }
}