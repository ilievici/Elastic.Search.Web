using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Elastic.Search.Core.Infrastructure.Abstract;
using Microsoft.Extensions.Configuration;

namespace Elastic.Search.Job.Infrastructure
{
    public class ConnectionProvider : IConnectionProvider
    {
        private string _clientId;
        private readonly IConfigurationRoot _configurationRoot;

        public ConnectionProvider(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IDbConnection OpenDbClientConnection()
        {
            return OpenSqlConnection(QueryClientConnectionString(_clientId));
        }

        public string GetClientId()
        {
            return _clientId;
        }

        public void SetClientId(string clientId)
        {
            _clientId = clientId;
        }
        
        private IDbConnection OpenDbMasterConnection()
        {
            var connString = _configurationRoot.GetConnectionString("MasterDB");
            return OpenSqlConnection(connString);
        }

        private string QueryClientConnectionString(string clientId)
        {
            string query = "SELECT TOP 1 SpeedpayConnectionString FROM dbo.ClientSite WHERE ClientSiteID = @clientId";
            string clientConnectionString;

            using (IDbConnection dbConnection = OpenDbMasterConnection())
            {
                IEnumerable<string> result = dbConnection.Query<string>(query, new { clientId });

                clientConnectionString = result.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(clientConnectionString))
                {
                    throw new ArgumentException($"Could not find client database connection string for {clientId}");
                }

                DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder(true)
                {
                    ConnectionString = clientConnectionString
                };

                if (dbConnectionStringBuilder.ContainsKey("provider"))
                {
                    dbConnectionStringBuilder.Remove("provider");
                }

                if (dbConnectionStringBuilder.ContainsKey("network library"))
                {
                    dbConnectionStringBuilder.Remove("network library");
                }

                var builder = new SqlConnectionStringBuilder
                {
                    ConnectionString = dbConnectionStringBuilder.ConnectionString
                };

                clientConnectionString = builder.ConnectionString;
            }

            return clientConnectionString;
        }

        private IDbConnection OpenSqlConnection(string connString)
        {
            IDbConnection dbConnection = new SqlConnection(connString);
            return dbConnection;
        }
    }
}
