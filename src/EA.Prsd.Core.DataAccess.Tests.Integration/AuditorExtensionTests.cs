namespace EA.Prsd.Core.DataAccess.Tests.Integration
{
    using EA.Prsd.Core.DataAccess.Extensions;
    using EA.Prsd.Core.DataAccess.Tests.Integration.Helpers;
    using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;
    using EA.Prsd.Core.Domain.Auditing;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AuditorExtensionTests
    {
        public AuditorExtensionTests()
        {
            var database = new TestDbContext();

            database.Database.CreateIfNotExists();

            database.Dispose();
        }

        [Fact]
        public void AuditChanges_InsertTwoSimpleEntities_AddsTwoAuditLogs_OfAddedType()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                SimpleEntity simple2 = new SimpleEntity("simple2");
                database.TestContext.SimpleEntities.AddRange(new List<SimpleEntity>() { simple1, simple2 });
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Two audit logs added
                var auditLogs = database.TestContext.AuditLogs;
                Assert.Equal(2, auditLogs.Count());

                //Both audit logs are additions
                List<EventType> auditTypes = auditLogs.Select(a => a.EventType).ToList<EventType>();
                Assert.All(auditTypes, e => Assert.Equal(EventType.Added, e));
            }
        }

        [Fact]
        public void AuditChanges_InsertAndModifySimpleEntity_AddsTwoAuditLogs_OfAddedAndModifiedTypes()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                database.TestContext.SimpleEntities.Add(simple1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                database.TestContext.SimpleEntities.Single().Data = "simple1 updated";
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Two audit logs added
                var auditLogs = database.TestContext.AuditLogs;
                Assert.Equal(2, auditLogs.Count());

                //Both audit logs are additions
                List<EventType> auditTypes = auditLogs.Select(a => a.EventType).ToList<EventType>();
                Assert.Equal(EventType.Added, auditTypes[0]);
                Assert.Equal(EventType.Modified, auditTypes[1]);
            }
        }

        [Fact]
        public void AuditChanges_InsertAndDeleteSimpleEntity_AddsTwoAuditLogs_OfAddedAndDeletedTypes()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                database.TestContext.SimpleEntities.Add(simple1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                database.TestContext.SimpleEntities.Remove(simple1);
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Two audit logs added
                var auditLogs = database.TestContext.AuditLogs;
                Assert.Equal(2, auditLogs.Count());

                //Both audit logs are additions
                List<EventType> auditTypes = auditLogs.Select(a => a.EventType).ToList<EventType>();
                Assert.Equal(EventType.Added, auditTypes[0]);
                Assert.Equal(EventType.Deleted, auditTypes[1]);
            }
        }

        [Fact]
        public void AuditChanges_InsertSimpleEntity_AddsAuditLogWithCorrectEntityId()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                database.TestContext.SimpleEntities.Add(simple1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Audit Log stores correct entity Id
                Guid simple1Id = database.TestContext.SimpleEntities.Single(se => se.Data == "simple1").Id;
                Assert.NotNull(database.TestContext.AuditLogs.SingleOrDefault(al => al.RecordId == simple1Id));
            }
        }

        [Fact]
        public void AuditChanges_InsertSimpleEntity_AddsAuditLogForCorrectTable()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                database.TestContext.SimpleEntities.Add(simple1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Audit Log stores correct table
                Guid simple1Id = database.TestContext.SimpleEntities.Single(se => se.Data == "simple1").Id;
                Assert.Matches("\\[Test\\]\\.\\[SimpleEntity\\]", 
                    database.TestContext.AuditLogs.Single(al => al.RecordId == simple1Id).TableName);
            }
        }

        [Fact]
        public void AuditChanges_InsertSimpleEntity_AddsAuditLogWithExpectedJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity simple1 = new SimpleEntity("simple1");
                database.TestContext.SimpleEntities.Add(simple1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Audit Log stores correct Json
                Guid simple1Id = database.TestContext.SimpleEntities.Single(se => se.Data == "simple1").Id;
                String expectedJson = "{\\\"Id\\\":\\\"" + simple1Id + "\\\",\\\"Data\\\":\\\"simple1\\\",\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == simple1Id).NewValue);
            }
        }

        [Fact]
        public void AuditChanges_InsertEntityWithEnums_AddsAuditLogWithExpectedEnumerationJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                EntityWithEnums ewe1 = new EntityWithEnums(TestEnumeration.B, SimpleEnum.Value1);
                database.TestContext.EnumEntities.Add(ewe1);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                database.TestContext.SaveChanges();

                //Assert
                //Audit Log stores correct Json, including the complex type
                Guid ewe1Id = database.TestContext.EnumEntities.Single().Id;
                String expectedJson = 
                    "{\\\"Id\\\":\\\"" + ewe1Id + "\\\",\\\"TestEnumerationValue\\\":\\\"{\\\\\"Value\\\\\":2}\\\",\\\"SimpleEnumValue\\\":1,\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == ewe1Id).NewValue);
            }
        }

        [Fact]
        public async Task AuditChanges_InsertEntityWithForeignId_AddsAuditLogWithExpectedForeignKeyJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity se = new SimpleEntity("Simple Entity");
                database.TestContext.SimpleEntities.Add(se);
                database.TestContext.SetEntityId();
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                EntityWithForeignId ewf = new EntityWithForeignId(se);
                database.TestContext.ForeignIdEntities.Add(ewf);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct json, with foreign key
                Guid foreignId = database.TestContext.ForeignIdEntities.Single().Id;
                Guid entityId = database.TestContext.SimpleEntities.Single(s => s.Data == "Simple Entity").Id;
                String expectedJson = "{\\\"Id\\\":\\\"" + foreignId + "\\\",\\\"SimpleEntityId\\\":\\\"" + entityId + "\\\",\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == foreignId).NewValue);
            }
        }

        [Fact(Skip = "Tests desired behaviour, but not actual behaviour. Currently fails, but full audits can be constructed by OriginalValues and current data instead")]
        public async Task AuditChanges_InsertEntityWithChildrenAndAddChildren_AddsAuditLogWithExpectedForeignKeyJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity sA = new SimpleEntity("Simple A");
                SimpleEntity sB = new SimpleEntity("Simple B");
                database.TestContext.SimpleEntities.AddRange(new List<SimpleEntity> { sA, sB });

                EntityWithChildren ewc = new EntityWithChildren(sA, sB);
                database.TestContext.ParentEntities.Add(ewc);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct json, with foreign keys
                Guid parentEntityId = database.TestContext.ParentEntities.Single().Id;
                Guid simpleEntityId = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B").Id;
                String expectedJson =
                    "{\\\"Id\\\":\\\"" + parentEntityId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + simpleEntityId + "\\\",\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == parentEntityId).NewValue);
            }
        }

        [Fact(Skip = "Tests desired behaviour, but not actual behaviour. Currently fails, but full audits can be constructed by OriginalValues and current data instead")]
        public async Task AuditChanges_InsertEntityWithChildrenAndUpdateChildren_AddsAuditLogWithExpectedNewForeignKeyJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                //Add initial entities
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity sA = new SimpleEntity("Simple A");
                SimpleEntity sB = new SimpleEntity("Simple B");
                database.TestContext.SimpleEntities.AddRange(new List<SimpleEntity> { sA, sB });
                EntityWithChildren ewc = new EntityWithChildren(sA, sB);
                database.TestContext.ParentEntities.Add(ewc);
                database.TestContext.SetEntityId();
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                // Update exisiting EntityWithChildren with new child entity
                SimpleEntity simpleEntityVersion2 = new SimpleEntity("Simple B Version 2");
                database.TestContext.SimpleEntities.Add(simpleEntityVersion2);
                ewc.UpdateSimpleEntityB(simpleEntityVersion2);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct new json, with foreign keys
                Guid parentEntityId = database.TestContext.ParentEntities.Single().Id;
                Guid simpeEntityVersion2Id = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B Version 2").Id;
                String expectedNewJson =
                    "{\\\"Id\\\":\\\"" + parentEntityId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + simpeEntityVersion2Id + "\\\",\\\"RowVersion\\\":.*}";
                List<AuditLog> orderedLogs = database.TestContext.AuditLogs.OrderBy(a => a.EventDate).ToList();
                Assert.Matches(expectedNewJson, orderedLogs.Last(al => al.RecordId == parentEntityId).NewValue);
            }
        }

            [Fact]
        public async Task AuditChanges_InsertEntityWithChildrenAndUpdateChildren_AddsAuditLogWithExpectedOriginalForeignKeyJson()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                //Arrange
                //Add initial entities
                Guid userId = Guid.Parse("F37FEB8F-A364-47F6-86A5-02E191E8AF20");
                SimpleEntity sA = new SimpleEntity("Simple A");
                SimpleEntity sB = new SimpleEntity("Simple B");
                database.TestContext.SimpleEntities.AddRange(new List<SimpleEntity> { sA, sB });
                EntityWithChildren ewc = new EntityWithChildren(sA, sB);
                database.TestContext.ParentEntities.Add(ewc);
                database.TestContext.SetEntityId();
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                // Update exisiting EntityWithChildren with new child entity
                SimpleEntity simpleEntityVersion2 = new SimpleEntity("Simple B Version 2");
                database.TestContext.SimpleEntities.Add(simpleEntityVersion2);
                ewc.UpdateSimpleEntityB(simpleEntityVersion2);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct original json, with foreign keys
                Guid parentEntityId = database.TestContext.ParentEntities.Single().Id;
                Guid simpleEntityId = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B").Id;
                String expectedOriginalJson =
                    "{\\\"Id\\\":\\\"" + parentEntityId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + simpleEntityId + "\\\",\\\"RowVersion\\\":.*}";
                List<AuditLog> orderedLogs = database.TestContext.AuditLogs.OrderBy(a => a.EventDate).ToList();
                Assert.Matches(expectedOriginalJson, orderedLogs.Last(al => al.RecordId == parentEntityId).OriginalValue);
            }
        }
    }
}
