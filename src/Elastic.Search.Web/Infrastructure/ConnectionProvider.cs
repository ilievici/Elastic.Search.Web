using System.Data;
using Elastic.Search.Core.Infrastructure.Abstract;

namespace Elastic.Search.Web.Infrastructure
{
    public class ConnectionProvider : IConnectionProvider
    {
        public IDbConnection OpenDbClientConnection()
        {
            throw new System.NotImplementedException();
        }

        public string GetClientId()
        { 
            //TODO: test values
            string clientId = "test_xo_cutexas_cprofile";
            return clientId.ToLower();
        }
    }
}
