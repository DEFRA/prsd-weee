/*
 * Modified from https://github.com/elmah/Elmah/blob/master/src/Elmah.SqlServer/SqlErrorLog.cs
 * 
 * ELMAH - Error Logging Modules and Handlers for ASP.NET
 * Copyright (c) 2004-9 Atif Aziz. All rights reserved.
 *
 *  Author(s):
 *
 *      Atif Aziz, http://www.raboof.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace EA.Weee.Api.Infrastructure.Infrastructure
{
    using EA.Weee.Api.Client.Entities;
    using EA.Weee.DataAccess;
    using Elmah;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class ElmahSqlLogger
    {
        private readonly WeeeContext context;

        public ElmahSqlLogger(WeeeContext context)
        {
            this.context = context;
        }

        public async Task Log(ErrorData errorData)
        {
            await
                context.Database.ExecuteSqlCommandAsync(
                    "ELMAH_LogError @ErrorId, @Application, @Host, @Type, @Source, @Message, @User, @AllXml, @StatusCode, @TimeUtc",
                    new SqlParameter("@ErrorId", errorData.Id),
                    new SqlParameter("@Application", errorData.ApplicationName),
                    new SqlParameter("@Host", errorData.HostName),
                    new SqlParameter("@Type", errorData.Type),
                    new SqlParameter("@Source", errorData.Source),
                    new SqlParameter("@Message", errorData.Message),
                    new SqlParameter("@User", errorData.User),
                    new SqlParameter("@AllXml", errorData.ErrorXml),
                    new SqlParameter("@StatusCode", errorData.StatusCode),
                    new SqlParameter("@TimeUtc", errorData.Date.ToUniversalTime()));
        }

        public async Task<ErrorData> GetError(Guid id, string applicationName)
        {
            var xml = await context.Database.SqlQuery<string>("ELMAH_GetErrorXml @Application, @ErrorId",
                new SqlParameter("@ErrorId", id),
                new SqlParameter("@Application", applicationName)).SingleOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(xml))
            {
                return null;
            }

            return CreateErrorDataFromXml(id, xml);
        }

        public async Task<PagedErrorDataList> GetPagedErrorList(int pageIndex, int pageSize, string applicationName)
        {
            var totalCountParameter = new SqlParameter("@TotalCount", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            int totalCount;
            ICollection<ErrorData> errorData;

            using (var command = context.Database.Connection.CreateCommand())
            {
                await command.Connection.OpenAsync();

                command.CommandText = "ELMAH_GetErrorsXml";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                    new SqlParameter("@Application", applicationName),
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    totalCountParameter
                });

                var xml = ReadSingleXmlStringResult(await command.ExecuteReaderAsync());
                errorData = ErrorsXmlToList(xml);

                totalCount = (int)command.Parameters["@TotalCount"].Value;

                command.Connection.Close();
            }

            return new PagedErrorDataList
            {
                Errors = errorData.ToArray(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                TotalRecords = totalCount
            };
        }

        private static string ReadSingleXmlStringResult(IDataReader reader)
        {
            using (reader)
            {
                if (!reader.Read())
                {
                    return null;
                }

                var sb = new StringBuilder(capacity: 2033);

                do
                {
                    sb.Append(reader.GetString(0));
                }
                while (reader.Read());

                return sb.ToString();
            }
        }

        private static ICollection<ErrorData> ErrorsXmlToList(string xml)
        {
            var errorEntryList = new List<ErrorData>();

            if (string.IsNullOrWhiteSpace(xml))
            {
                return errorEntryList;
            }

            var settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            using (var reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.IsStartElement("error"))
                {
                    var id = reader.GetAttribute("errorId");
                    var errorXml = reader.ReadOuterXml();

                    errorEntryList.Add(CreateErrorDataFromXml(new Guid(id), errorXml));
                }

                return errorEntryList;
            }
        }

        private static ErrorData CreateErrorDataFromXml(Guid id, string xml)
        {
            var error = ErrorXml.DecodeString(xml);
            return new ErrorData(id, error.ApplicationName, error.HostName,
                error.Type, error.Source, error.Message, error.User, error.StatusCode, error.Time, xml);
        }
    }
}