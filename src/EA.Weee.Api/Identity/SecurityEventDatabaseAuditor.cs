namespace EA.Weee.Api.Identity
{
    using EA.Prsd.Core;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// An implementation of the ISecurityEventAuditor interface which
    /// saves security event audit information to a database table.
    /// </summary>
    public class SecurityEventDatabaseAuditor : ISecurityEventAuditor
    {
        private readonly string connectionString;

        public SecurityEventDatabaseAuditor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task LoginSuccess(string userId)
        {
            await LogEvent(0, "LoginSuccess", userId, null);
        }

        public async Task LoginFailure(string userName)
        {
            string data = JsonConvert.SerializeObject(new { userName });
            await LogEvent(1, "LoginFailure", null, data);
        }

        public async Task UserCreated(string userId)
        {
            await LogEvent(2, "UserCreated", userId, null);
        }

        public async Task PasswordReset(string userId)
        {
            await LogEvent(3, "PasswordReset", userId, null);
        }

        public async Task EmailConfirmed(string userId)
        {
            await LogEvent(4, "EmailConfirmed", userId, null);
        }

        private async Task LogEvent(int eventId, string eventName, string userId, string data)
        {
            Guard.ArgumentNotNullOrEmpty(() => eventName, eventName);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = System.Data.CommandType.Text;
                    cm.CommandText = @"
                        INSERT INTO [Auditing].[AuditEvent]
                               ([EventDate]
                               ,[Scope]
                               ,[EventId]
                               ,[EventName]
                               ,[UserId]
                               ,[Data])
                         VALUES
                               (@EventDate
                               ,@Scope
                               ,@EventId
                               ,@EventName
                               ,@UserId
                               ,@Data)";
                    cm.Parameters.AddWithValue("@EventDate", DateTime.UtcNow);
                    cm.Parameters.AddWithValue("@Scope", "Security");
                    cm.Parameters.AddWithValue("@EventId", eventId);
                    cm.Parameters.AddWithValue("@EventName", eventName);
                    cm.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                    cm.Parameters.AddWithValue("@Data", (object)data ?? DBNull.Value);

                    await cm.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
