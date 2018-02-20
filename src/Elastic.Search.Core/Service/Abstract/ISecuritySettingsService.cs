using System.Collections.Generic;

namespace Elastic.Search.Core.Service.Abstract
{
    public interface ISecuritySettingsService
    {
        FiledSettingsCollection GetSettings();
    }
}