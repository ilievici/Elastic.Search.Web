using System.Collections.Generic;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;

namespace Elastic.Search.Core.Service
{
    public class SecuritySettingsService : ISecuritySettingsService
    {
        public FiledSettingsCollection GetSettings()
        {
            var settings = new List<FiledSettings>
            {
                new FiledSettings(nameof(ElasticPaymentModel.CreditAccount), true, true),
                new FiledSettings(nameof(ElasticPaymentModel.Confirmation), false, true),

                new FiledSettings(nameof(ElasticPaymentModel.FirstName), false, false),
                new FiledSettings(nameof(ElasticPaymentModel.LastName), false, false),

                new FiledSettings(nameof(ElasticPaymentModel.DebitAccount), true, true),

                new FiledSettings(nameof(ElasticPaymentModel.Email),false, true),
                //new FiledSettings(f => f.Phone, true, false),

                //new FiledSettings(f => f.PaymentAmountFrom, false, true),
                //new FiledSettings(f => f.PaymentAmountTo, false, true),

                //new FiledSettings(f => f.DateType, false, true),
                //new FiledSettings(f => f.PaymentStartDate, false, true),
                //new FiledSettings(f => f.PaymentEndDate, false, true)
            };

            return new FiledSettingsCollection(settings);
        }
    }
}
