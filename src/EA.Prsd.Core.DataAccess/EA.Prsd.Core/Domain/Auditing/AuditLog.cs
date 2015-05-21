namespace EA.Prsd.Core.Domain.Auditing
{
    using System;

    public class AuditLog
    {
        private AuditLog()
        {
        }

        public AuditLog(Guid userId, DateTime eventDate, EventType eventType, string tableName, Guid recordId,
            string originalValue = null, string newValue = null)
        {
            Guard.ArgumentNotDefaultValue(() => userId, userId);
            Guard.ArgumentNotDefaultValue(() => eventDate, eventDate);
            Guard.ArgumentNotNull(tableName);
            Guard.ArgumentNotDefaultValue(() => recordId, recordId);

            UserId = userId;
            EventDate = eventDate;
            EventType = eventType;
            TableName = tableName;
            RecordId = recordId;
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public int Id { get; private set; }

        public Guid UserId { get; private set; }

        public DateTime EventDate { get; private set; }

        public EventType EventType { get; private set; }

        public string TableName { get; private set; }

        public Guid RecordId { get; private set; }

        public string OriginalValue { get; private set; }

        public string NewValue { get; private set; }
    }
}