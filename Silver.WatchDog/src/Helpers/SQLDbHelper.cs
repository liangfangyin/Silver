﻿using Dapper;
using System.Data;
using System.Threading.Tasks;
using  Silver.WatchDog.src.Data;
using  Silver.WatchDog.src.Models;
using  Silver.WatchDog.src.Utilities;

namespace  Silver.WatchDog.src.Helpers
{
    internal static class SQLDbHelper
    {
        // WATCHLOG OPERATIONS
        public static async Task<Page<WatchLog>> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber)
        {
            var query = @$"SELECT * FROM {Constants.WatchLogTableName} ";

            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(verbString) || !string.IsNullOrEmpty(statusCode))
                query += "WHERE ";

            if (!string.IsNullOrEmpty(searchString))
            {
                if(GeneralHelper.IsPostgres())
                    query += $"({nameof(WatchLog.Path)} LIKE '%{searchString}%' OR {nameof(WatchLog.Method)} LIKE '%{searchString}%' OR {nameof(WatchLog.ResponseStatus)}::text LIKE '%{searchString}%' OR {nameof(WatchLog.QueryString)} LIKE '%{searchString}%')" + (string.IsNullOrEmpty(statusCode) && string.IsNullOrEmpty(verbString) ? "" : " AND ");
                else
                    query += $"({nameof(WatchLog.Path)} LIKE '%{searchString}%' OR {nameof(WatchLog.Method)} LIKE '%{searchString}%' OR {nameof(WatchLog.ResponseStatus)} LIKE '%{searchString}%' OR {nameof(WatchLog.QueryString)} LIKE '%{searchString}%')" + (string.IsNullOrEmpty(statusCode) && string.IsNullOrEmpty(verbString) ? "" : " AND ");
            }

            if (!string.IsNullOrEmpty(verbString))
            {
                query += $"{nameof(WatchLog.Method)} LIKE '%{verbString}%' " + (string.IsNullOrEmpty(statusCode) ? "" : "AND ");
            }

            if (!string.IsNullOrEmpty(statusCode))
            {
                query += $"{nameof(WatchLog.ResponseStatus)} = {statusCode}";
            }
            query += $" ORDER BY {nameof(WatchLog.Id)} DESC";
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                var logs = await connection.QueryAsync<WatchLog>(query);
                connection.Close();
                return logs.ToPaginatedList(pageNumber);
            }
        }

        public static async Task InsertWatchLog(WatchLog log)
        {
            bool isPostgres = GeneralHelper.IsPostgres();
            var query = @$"INSERT INTO {Constants.WatchLogTableName} (responseBody,responseStatus,requestBody,queryString,path,requestHeaders,responseHeaders,method,host,ipAddress,timeSpent,startTime,endTime) " +
                "VALUES (@ResponseBody,@ResponseStatus,@RequestBody,@QueryString,@Path,@RequestHeaders,@ResponseHeaders,@Method,@Host,@IpAddress,@TimeSpent,@StartTime,@EndTime);";

            var parameters = new DynamicParameters();
            parameters.Add("ResponseBody", isPostgres ? log.ResponseBody.Replace("\u0000", "") : log.ResponseBody, DbType.String);
            parameters.Add("ResponseStatus", log.ResponseStatus, DbType.Int32);
            parameters.Add("RequestBody", isPostgres ? log.RequestBody.Replace("\u0000", "") : log.RequestBody, DbType.String);
            parameters.Add("QueryString", log.QueryString, DbType.String);
            parameters.Add("Path", log.Path, DbType.String);
            parameters.Add("RequestHeaders", log.RequestHeaders, DbType.String);
            parameters.Add("ResponseHeaders", log.ResponseHeaders, DbType.String);
            parameters.Add("Method", log.Method, DbType.String);
            parameters.Add("Host", log.Host, DbType.String);
            parameters.Add("IpAddress", log.IpAddress, DbType.String);
            parameters.Add("TimeSpent", log.TimeSpent, DbType.String);

            if (isPostgres)
            {
                parameters.Add("StartTime", log.StartTime.ToUniversalTime(), DbType.DateTime);
                parameters.Add("EndTime", log.EndTime.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("StartTime", log.StartTime);
                parameters.Add("EndTime", log.EndTime);
            }

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                await connection.ExecuteAsync(query, parameters);
                connection.Close();
            }
        }

        public static async Task BatchInsertWatchLog(List<WatchLog> log)
        {
            if (log.Count <= 0)
            {
                return;
            }
            bool isPostgres = GeneralHelper.IsPostgres();
            int number = 0;
            var query = @$"INSERT INTO {Constants.WatchLogTableName} (responseBody,responseStatus,requestBody,queryString,path,requestHeaders,responseHeaders,method,host,ipAddress,timeSpent,startTime,endTime) VALUES";
            var parameters = new DynamicParameters();
            foreach (var item in log)
            {
                if (number != 0)
                {
                    query += ",";
                }
                query += "(@ResponseBody"+ number + ",@ResponseStatus"+ number + ",@RequestBody"+ number + ",@QueryString"+ number + ",@Path"+ number + ",@RequestHeaders"+ number + ",@ResponseHeaders"+ number + ",@Method"+ number + ",@Host"+ number + ",@IpAddress"+ number + ",@TimeSpent"+ number + ",@StartTime"+ number + ",@EndTime"+ number + ")";
                parameters.Add("ResponseBody"+ number, isPostgres ? item.ResponseBody.Replace("\u0000", "") : item.ResponseBody, DbType.String);
                parameters.Add("ResponseStatus" + number, item.ResponseStatus, DbType.Int32);
                parameters.Add("RequestBody" + number, isPostgres ? item.RequestBody.Replace("\u0000", "") : item.RequestBody, DbType.String);
                parameters.Add("QueryString" + number, item.QueryString, DbType.String);
                parameters.Add("Path" + number, item.Path, DbType.String);
                parameters.Add("RequestHeaders" + number, item.RequestHeaders, DbType.String);
                parameters.Add("ResponseHeaders" + number, item.ResponseHeaders, DbType.String);
                parameters.Add("Method" + number, item.Method, DbType.String);
                parameters.Add("Host" + number, item.Host, DbType.String);
                parameters.Add("IpAddress" + number, item.IpAddress, DbType.String);
                parameters.Add("TimeSpent" + number, item.TimeSpent, DbType.String);

                if (isPostgres)
                {
                    parameters.Add("StartTime" + number, item.StartTime.ToUniversalTime(), DbType.DateTime);
                    parameters.Add("EndTime" + number, item.EndTime.ToUniversalTime(), DbType.DateTime);
                }
                else
                {
                    parameters.Add("StartTime" + number, item.StartTime);
                    parameters.Add("EndTime" + number, item.EndTime);
                }
                number++;
            }
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                await connection.ExecuteAsync(query, parameters);
                connection.Close();
            }
        }


        // WATCH EXCEPTION OPERATIONS
        public static async Task<Page<WatchExceptionLog>> GetAllWatchExceptionLogs(string searchString, int pageNumber)
        {
            var query = @$"SELECT * FROM {Constants.WatchLogExceptionTableName} ";
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query += $"WHERE {nameof(WatchExceptionLog.Source)} LIKE '%{searchString}%' OR {nameof(WatchExceptionLog.Message)} LIKE '%{searchString}%' OR {nameof(WatchExceptionLog.StackTrace)} LIKE '%{searchString}%' ";
            }
            query += $"ORDER BY {nameof(WatchExceptionLog.Id)} DESC";
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                var logs = await connection.QueryAsync<WatchExceptionLog>(query);
                return logs.ToPaginatedList(pageNumber);
            }
        }

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            var query = @$"INSERT INTO {Constants.WatchLogExceptionTableName} (message,stackTrace,typeOf,source,path,method,queryString,requestBody,encounteredAt) " +
                "VALUES (@Message,@StackTrace,@TypeOf,@Source,@Path,@Method,@QueryString,@RequestBody,@EncounteredAt);";

            var parameters = new DynamicParameters();
            parameters.Add("Message", log.Message, DbType.String);
            parameters.Add("StackTrace", log.StackTrace, DbType.String);
            parameters.Add("TypeOf", log.TypeOf, DbType.String);
            parameters.Add("Source", log.Source, DbType.String);
            parameters.Add("Path", log.Path, DbType.String);
            parameters.Add("Method", log.Method, DbType.String);
            parameters.Add("QueryString", log.QueryString, DbType.String);
            parameters.Add("RequestBody", log.RequestBody, DbType.String);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("EncounteredAt", log.EncounteredAt.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("EncounteredAt", log.EncounteredAt, DbType.DateTime);
            }

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public static async Task BatchInsertWatchExceptionLog(List<WatchExceptionLog> log)
        {
            if (log.Count <= 0)
            {
                return;
            }
            int number = 0;
            var query = @$"INSERT INTO {Constants.WatchLogExceptionTableName} (message,stackTrace,typeOf,source,path,method,queryString,requestBody,encounteredAt) VALUES";
            var parameters = new DynamicParameters();
            foreach (var item in log)
            {
                if (number != 0)
                {
                    query += ",";
                }
                query += "(@Message"+ number + ",@StackTrace"+ number + ",@TypeOf"+ number + ",@Source"+ number + ",@Path"+ number + ",@Method"+ number + ",@QueryString"+ number + ",@RequestBody"+ number + ",@EncounteredAt"+ number + ")";
                parameters.Add("Message" + number, item.Message, DbType.String);
                parameters.Add("StackTrace" + number, item.StackTrace, DbType.String);
                parameters.Add("TypeOf" + number, item.TypeOf, DbType.String);
                parameters.Add("Source" + number, item.Source, DbType.String);
                parameters.Add("Path" + number, item.Path, DbType.String);
                parameters.Add("Method" + number, item.Method, DbType.String);
                parameters.Add("QueryString" + number, item.QueryString, DbType.String);
                parameters.Add("RequestBody" + number, item.RequestBody, DbType.String);
                if (GeneralHelper.IsPostgres())
                {
                    parameters.Add("EncounteredAt" + number, item.EncounteredAt.ToUniversalTime(), DbType.DateTime);
                }
                else
                {
                    parameters.Add("EncounteredAt" + number, item.EncounteredAt, DbType.DateTime);
                }
                number++;
            }
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        // LOGS OPERATION
        public static async Task<Page<WatchLoggerModel>> GetAllLogs(string searchString, string logLevelString, int pageNumber)
        {
            var query = @$"SELECT * FROM {Constants.LogsTableName} ";

            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(logLevelString))
                query += "WHERE ";

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query += $"{nameof(WatchLoggerModel.CallingFrom)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.CallingMethod)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.Message)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.EventId)} LIKE '%{searchString}%' " + (string.IsNullOrEmpty(logLevelString) ? "" : "AND ");
            }

            if (!string.IsNullOrEmpty(logLevelString))
            {
                query += $"{nameof(WatchLoggerModel.LogLevel)} LIKE '%{logLevelString}%' ";
            }
            query += $"ORDER BY {nameof(WatchLoggerModel.Id)} DESC";

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                var logs = await connection.QueryAsync<WatchLoggerModel>(query);
                connection.Close();
                return logs.ToPaginatedList(pageNumber);
            }
        }

        public static async Task InsertLog(WatchLoggerModel log)
        {
            var query = @$"INSERT INTO {Constants.LogsTableName} (message,eventId,timestamp,callingFrom,callingMethod,lineNumber,logLevel) " +
                "VALUES (@Message,@EventId,@Timestamp,@CallingFrom,@CallingMethod,@LineNumber,@LogLevel);";

            var parameters = new DynamicParameters();
            parameters.Add("Message", log.Message, DbType.String);
            parameters.Add("CallingFrom", log.CallingFrom, DbType.String);
            parameters.Add("CallingMethod", log.CallingMethod, DbType.String);
            parameters.Add("LineNumber", log.LineNumber, DbType.Int32);
            parameters.Add("LogLevel", log.LogLevel, DbType.String);
            parameters.Add("EventId", log.EventId, DbType.String);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("Timestamp", log.Timestamp.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("Timestamp", log.Timestamp, DbType.DateTime);
            }

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public static async Task BatchInsertLog(List<WatchLoggerModel> log)
        {
            if (log.Count <= 0)
            {
                return;
            }
            int number = 0;
            var query = @$"INSERT INTO {Constants.LogsTableName} (message,eventId,timestamp,callingFrom,callingMethod,lineNumber,logLevel) VALUES";
            var parameters = new DynamicParameters();
            foreach (var item in log)
            {
                if (number != 0)
                {
                    query += ",";
                }
                query += "(@Message"+ number + ",@EventId"+ number + ",@Timestamp"+ number + ",@CallingFrom"+ number + ",@CallingMethod"+ number + ",@LineNumber"+ number + ",@LogLevel"+ number + ")";
                parameters.Add("Message"+ number, item.Message, DbType.String);
                parameters.Add("CallingFrom" + number, item.CallingFrom, DbType.String);
                parameters.Add("CallingMethod" + number, item.CallingMethod, DbType.String);
                parameters.Add("LineNumber" + number, item.LineNumber, DbType.Int32);
                parameters.Add("LogLevel" + number, item.LogLevel, DbType.String);
                parameters.Add("EventId" + number, item.EventId, DbType.String);
                if (GeneralHelper.IsPostgres())
                {
                    parameters.Add("Timestamp" + number, item.Timestamp.ToUniversalTime(), DbType.DateTime);
                }
                else
                {
                    parameters.Add("Timestamp" + number, item.Timestamp, DbType.DateTime);
                }
                number++;
            }
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }


        public static async Task<bool> ClearLogs()
        {
            var watchlogQuery = @$"truncate table {Constants.WatchLogTableName}";
            var exQuery = @$"truncate table {Constants.WatchLogExceptionTableName}";
            var logQuery = @$"truncate table {Constants.LogsTableName}";
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                var watchlogs = await connection.ExecuteAsync(watchlogQuery);
                var exLogs = await connection.ExecuteAsync(exQuery);
                var logs = await connection.ExecuteAsync(logQuery);
                return watchlogs > 1 && exLogs > 1 && logs > 1;
            }
        }
    }
}
