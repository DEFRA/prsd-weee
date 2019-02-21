namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;

    public interface IOperatorEntity<T> where T : Entity
    {
        Operator Operator { get; }
    }
}
