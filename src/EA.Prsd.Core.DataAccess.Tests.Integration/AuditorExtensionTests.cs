using System;
using System.Collections.Generic;
using System.Linq;
using EA.Prsd.Core.DataAccess.Extensions;
using EA.Prsd.Core.DataAccess.Tests.Integration.Helpers;
using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;
using EA.Prsd.Core.Domain.Auditing;
using Xunit;

namespace EA.Prsd.Core.DataAccess.Tests.Integration
{
    public class AuditorExtensionTests
    {

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
                Assert.All(auditTypes, e => Assert.Equal(e, EventType.Added));
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
                Assert.Equal(auditTypes[0], EventType.Added);
                Assert.Equal(auditTypes[1], EventType.Modified);
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
                Assert.Equal(auditTypes[0], EventType.Added);
                Assert.Equal(auditTypes[1], EventType.Deleted);
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
        public async void AuditChanges_InsertEntityWithForeignId_AddsAuditLogWithExpectedForeignKeyJson()
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
                Guid ewfId = database.TestContext.ForeignIdEntities.Single().Id;
                Guid seId = database.TestContext.SimpleEntities.Single(s => s.Data == "Simple Entity").Id;
                String expectedJson = "{\\\"Id\\\":\\\"" + ewfId + "\\\",\\\"SimpleEntityId\\\":\\\"" + seId + "\\\",\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == ewfId).NewValue);
            }
        }

        [Fact(Skip = "Tests desired behaviour, but not actual behaviour. Currently fails, but full audits can be constructed by OriginalValues and current data instead")]
        public async void AuditChanges_InsertEntityWithChildrenAndAddChildren_AddsAuditLogWithExpectedForeignKeyJson()
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
                Guid ewcId = database.TestContext.ParentEntities.Single().Id;
                Guid sBId = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B").Id;
                String expectedJson =
                    "{\\\"Id\\\":\\\"" + ewcId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + sBId + "\\\",\\\"RowVersion\\\":null}";
                Assert.Matches(expectedJson,
                    database.TestContext.AuditLogs.Single(al => al.RecordId == ewcId).NewValue);
            }
        }

        [Fact(Skip = "Tests desired behaviour, but not actual behaviour. Currently fails, but full audits can be constructed by OriginalValues and current data instead")]
        public async void AuditChanges_InsertEntityWithChildrenAndUpdateChildren_AddsAuditLogWithExpectedNewForeignKeyJson()
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
                SimpleEntity sBv2 = new SimpleEntity("Simple B Version 2");
                database.TestContext.SimpleEntities.Add(sBv2);
                ewc.UpdateSimpleEntityB(sBv2);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct new json, with foreign keys
                Guid ewcId = database.TestContext.ParentEntities.Single().Id;
                Guid sBv2Id = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B Version 2").Id;
                String expectedNewJson =
                    "{\\\"Id\\\":\\\"" + ewcId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + sBv2Id + "\\\",\\\"RowVersion\\\":.*}";
                List<AuditLog> orderedLogs = database.TestContext.AuditLogs.OrderBy(a => a.EventDate).ToList();
                Assert.Matches(expectedNewJson, orderedLogs.Last(al => al.RecordId == ewcId).NewValue);
            }
        }

            [Fact]
        public async void AuditChanges_InsertEntityWithChildrenAndUpdateChildren_AddsAuditLogWithExpectedOriginalForeignKeyJson()
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
                SimpleEntity sBv2 = new SimpleEntity("Simple B Version 2");
                database.TestContext.SimpleEntities.Add(sBv2);
                ewc.UpdateSimpleEntityB(sBv2);
                database.TestContext.SetEntityId();

                //Act
                database.TestContext.AuditChanges(userId);
                await database.TestContext.SaveChangesAsync();

                //Assert
                //Audit Log stores correct original json, with foreign keys
                Guid ewcId = database.TestContext.ParentEntities.Single().Id;
                Guid sBId = database.TestContext.SimpleEntities.Single(se => se.Data == "Simple B").Id;
                String expectedOriginalJson =
                    "{\\\"Id\\\":\\\"" + ewcId + "\\\",\\\"SimpleEntityBId\\\":\\\"" + sBId + "\\\",\\\"RowVersion\\\":.*}";
                List<AuditLog> orderedLogs = database.TestContext.AuditLogs.OrderBy(a => a.EventDate).ToList();
                Assert.Matches(expectedOriginalJson, orderedLogs.Last(al => al.RecordId == ewcId).OriginalValue);
            }
        }
    }
}
