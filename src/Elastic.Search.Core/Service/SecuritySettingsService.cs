using System.Collections.Generic;
using Elastic.Search.Core.Extensions;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;

namespace Elastic.Search.Core.Service
{
    public class SecuritySettingsService : ISecuritySettingsService
    {
        //TODO: Read this data from manifest
        public FiledSettingsCollection GetSettings()
        {
            var settings = new List<FiledSettings>
            {
                new FiledSettings(nameof(PaymentFilterModel.CreditAccount), true, true),
                new FiledSettings(nameof(PaymentFilterModel.DebitAccount), true, true),

                new FiledSettings(nameof(PaymentFilterModel.Confirmation), false, true),

                new FiledSettings(nameof(PaymentFilterModel.FirstName), false, false),
                new FiledSettings(nameof(PaymentFilterModel.LastName), false, false),

                new FiledSettings(nameof(PaymentFilterModel.Email), false, true),
                new FiledSettings(nameof(PaymentFilterModel.Phone), false, true),

                new FiledSettings(nameof(PaymentFilterModel.PaymentAmountFrom), false, true),
                new FiledSettings(nameof(PaymentFilterModel.PaymentAmountTo), false, true),

                new FiledSettings(nameof(PaymentFilterModel.DateType), false, true),
                new FiledSettings(nameof(PaymentFilterModel.PaymentStartDate), false, true),
                new FiledSettings(nameof(PaymentFilterModel.PaymentEndDate), false, true),
                
                new FiledSettings(nameof(PaymentFilterModel.RetreiveSelfEnteredPayments), false, true),
                new FiledSettings(nameof(PaymentFilterModel.RetreiveSiteSpecificPayments), false, true)
            };

            return new FiledSettingsCollection(settings);
        }
    }
}
