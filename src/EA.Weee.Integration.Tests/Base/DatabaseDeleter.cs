namespace EA.Weee.Integration.Tests.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using EA.Weee.Core.Helpers;

    internal class DatabaseDeleter
    {
        private class Relationship
        {
            public string PrimaryKeyTable { get; set; }
            public string ForeignKeyTable { get; set; }
        }

        private class NotInitializedException : Exception
        {
            public NotInitializedException(string database)
                : base($"<{database}> has not been initialized")
            {
            }
        }

        private static readonly object LockObj = new object();
        private static readonly string[] IgnoredTables =
        {
        "[Auditing].[AuditEvent]",
        "[Auditing].[AuditLog]",
        "[dbo].[__MigrationHistory]",
        "[dbo].[SystemData]",
        "[dbo].[usd_AppliedDatabaseScript]",
        "[dbo].[usd_AppliedDatabaseTestDataScript]",
        "[Identity].[AspNetRoles]",
        "[Identity].[AspNetUserClaims]",
        "[Identity].[AspNetUserLogins]",
        "[Identity].[AspNetUserRoles]",
        "[Identity].[AspNetUsers]",
        "[Logging].[ELMAH_Error]",
        "[Lookup].[ChargeBandAmount]",
        "[Lookup].[CompetentAuthority]",
        "[Lookup].[Country]",
        "[Lookup].[EvidenceNoteProtocol]",
        "[Lookup].[EvidenceNoteStatus]",
        "[Lookup].[EvidenceNoteWasteType]",
        "[Lookup].[LocalArea]",
        "[Lookup].[PanArea]",
        "[Lookup].[QuarterWindowTemplate]",
        "[Lookup].[WeeeCategory]",
        "[Security].[Role]"
        };

        private static readonly TableData[] DeleteTables =
        {
            new TableData()
            {
                Schema = "Evidence",
                TableName = "NoteStatusHistory"
            },
            new TableData()
            {
                Schema = "Evidence",
                TableName = "NoteTransferTonnage"
            },
            new TableData()
            {
                Schema = "Evidence",
                TableName = "NoteTonnage"
            },
            new TableData()
            {
                Schema = "Evidence",
                TableName = "Note"
            },
            new TableData()
            {
                Schema = "PCS",
                TableName = "ObligationSchemeAmount"
            },
            new TableData()
            {
                Schema = "PCS",
                TableName = "ObligationScheme"
            },
            new TableData()
            {
                Schema = "PCS",
                TableName = "ObligationUploadError"
            },
            new TableData()
            {
                Schema = "PCS",
                TableName = "ObligationUpload"
            },
            new TableData()
            {
                Schema = "PCS",
                TableName = "Scheme"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeReceivedAmount"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeReusedAmount"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeSentOnAmount"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeSentOn"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeReusedSite"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeReused"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "WeeeReceived"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "ReturnScheme"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "ReturnReportOn"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "ReturnAatf"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "ReportOnQuestion"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "NonObligatedWeee"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "Return"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "AATF"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "Contact"
            },
            new TableData()
            {
                Schema = "AATF",
                TableName = "Address"
            },
            new TableData()
            {
                Schema = "Organisation",
                TableName = "OrganisationUser"
            },
        };

        private readonly string connectionString;
        private readonly bool useTruncate;
        private IEnumerable<TableData> tablesToDelete;
        private readonly Dictionary<string, string> deleteSql = new Dictionary<string, string>();
        private readonly HashSet<string> initialized = new HashSet<string>();

        public DatabaseDeleter(string connectionString, bool useTruncate)
        {
            this.connectionString = connectionString;
            this.useTruncate = useTruncate;

            BuildDeleteTables();
        }

        public void DeleteAllData()
        {
            if (!this.deleteSql.TryGetValue(connectionString, out var deleteSql))
            {
                throw new NotInitializedException(connectionString);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteSql;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void BuildDeleteTables()
        {
            if (initialized.Contains(connectionString))
            {
                return;
            }

            lock (LockObj)
            {
                if (!initialized.Add(connectionString))
                {
                    return;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var allTables = GetAllTables(connection);

                    //var allRelationships = GetRelationships(connection);

                    tablesToDelete = allTables; //BuildTableList(allTables, allRelationships);

                    deleteSql[connectionString] = BuildTableSql(tablesToDelete, useTruncate);
                }
            }
        }

        private static string BuildTableSql(IEnumerable<TableData> tablesToDelete, bool useTruncate)
        {
            return string.Join(" ",
                tablesToDelete.Select(t => useTruncate
                    ? $"truncate table [{t.Schema}].[{t.TableName}]"
                    : $"delete from [{t.Schema}].[{t.TableName}]; " +
                      (t.CurrentIdent.HasValue && t.CurrentIdent.Value > 0 ? 
                          $"DBCC CHECKIDENT ('[{t.Schema}].[{t.TableName}]', RESEED, 0); " : string.Empty)));
        }

        private static IEnumerable<TableData> BuildTableList(ICollection<TableData> allTables,
            ICollection<Relationship> allRelationships)
        {
            var tablesToDelete = new List<TableData>();

            while (allTables.Any())
            {
                var leafTables = allTables
                    .Where(t => allRelationships.All(r => r.PrimaryKeyTable != t.TableName))
                    .ToArray();

                tablesToDelete.AddRange(leafTables);

                foreach (var leafTable in leafTables)
                {
                    allTables.Remove(leafTable);

                    var relToRemove = allRelationships.Where(rel => rel.ForeignKeyTable == leafTable.TableName)
                        .ToArray();

                    foreach (var rel in relToRemove)
                    {
                        allRelationships.Remove(rel);
                    }
                }
            }

            return tablesToDelete;
        }

        private static IList<Relationship> GetRelationships(IDbConnection connection)
        {
            var relationships = new List<Relationship>();
            using (var query = connection.CreateCommand())
            {
                query.CommandText = @"select
                                        so_pk.name as PrimaryKeyTable
                                    ,   so_fk.name as ForeignKeyTable
                                    from
                                         sysforeignkeys sfk
                                         inner join sysobjects so_pk on sfk.rkeyid = so_pk.id
                                         inner join sysobjects so_fk on sfk.fkeyid = so_fk.id
                                    where so_pk.name <> so_fk.name
                                    order by
                                             so_pk.name
                                            ,so_fk.name";

                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var primaryKeyTable = reader.GetString(0);
                        var foreignKeyTable = reader.GetString(1);
                        relationships.Add(new Relationship
                        {
                            PrimaryKeyTable = primaryKeyTable,
                            ForeignKeyTable = foreignKeyTable
                        });
                    }
                }
                return relationships;
            }
        }

        private static IList<TableData> GetAllTables(IDbConnection connection)
        {
            using (var query = connection.CreateCommand())
            {
                query.CommandText =
                    @"  select '[' + s.Name  + '].' + '[' + t.name + ']', 
                        IDENT_SEED('[' + s.Name + '].' + '[' + t.name + ']'), IDENT_CURRENT('[' + s.Name + '].' + '[' + t.name + ']')
                    from sys.tables t
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                    WHERE s.name IN ('dbo', 'AATF', 'Admin', 'Charging', 'Evidence', 'Organisation', 'PCS', 'Producer')";
                var tables = new List<TableData>();
                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var identSeed = reader.IsDBNull(1) ? (int?)null : reader.GetDecimal(1).ToInt();
                        var identCurrent = reader.IsDBNull(2) ? (int?)null : reader.GetDecimal(2).ToInt();
                        tables.Add(new TableData
                        {
                            TableName = name,
                            IdentSeed = identSeed,
                            CurrentIdent = identCurrent
                        });
                    }
                }

                foreach (var tableData in tables)
                {
                    var updateTableData = DeleteTables.FirstOrDefault(t => ($"[{t.Schema}].[{t.TableName}]").Equals(tableData.TableName));
                    if (updateTableData != null)
                    {
                        updateTableData.CurrentIdent = tableData.CurrentIdent;
                        updateTableData.IdentSeed = tableData.IdentSeed;
                    }
                }

                return DeleteTables.Where(t => IgnoredTables.All(it => it != t.TableName)).ToList();
            }
        }

        private class TableData
        {
            public string Schema { get; set; }
            public string TableName { get; set; }
            public int? IdentSeed { get; set; }
            public int? CurrentIdent { get; set; }
        }
    }
}
