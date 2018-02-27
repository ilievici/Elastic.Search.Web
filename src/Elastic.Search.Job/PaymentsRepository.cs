using System.Collections.Generic;
using System.Linq;
using Dapper;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;

namespace Elastic.Search.Job
{
    public interface IPaymentsRepository
    {
        int GetTotalPayments(int lastPaymentId);

        IList<ElasticPaymentModel> GetPayments(string sql, int startRow, int endRow, int lastPaymentId);
    }

    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly IConnectionProvider _connectionProvider;

        public PaymentsRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IList<ElasticPaymentModel> GetPayments(string sql, int startRow, int endRow, int lastPaymentId)
        {
            using (var connection = _connectionProvider.OpenDbClientConnection())
            {
                return connection.Query<ElasticPaymentModel>(string.Format(sql, startRow, endRow, lastPaymentId))
                    .ToList();
            }
        }

        public int GetTotalPayments(int lastPaymentId)
        {
            using (var connection = _connectionProvider.OpenDbClientConnection())
            {
                return connection.Query<int>($"SELECT COUNT(*) FROM [dbo].[PAYMENTS] WHERE [PAYMENT_ID] > {lastPaymentId}")
                    .FirstOrDefault();
            }
        }
    }
}
