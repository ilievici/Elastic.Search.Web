using System.Collections.Generic;
using System.Linq;
using Dapper;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;

namespace Elastic.Search.Job
{
    public interface IPaymentsRepository
    {
        IList<ElasticPaymentModel> GetPayments(string sql);
    }

    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public PaymentsRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IList<ElasticPaymentModel> GetPayments(string sql)
        {
            using (var connection = _connectionProvider.OpenDbClientConnection())
            {
                return connection.Query<ElasticPaymentModel>(sql)
                    .ToList();
            }
        }
    }
}
