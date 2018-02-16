using System.Data;

namespace Elastic.Search.Core.Infrastructure.Abstract
{
    public interface IConnectionProvider
    {
        IDbConnection OpenDbClientConnection();

        string GetClientId();
    }
}
