using Elastic.Search.Core.Extensions;

namespace Elastic.Search.Core.Service.Abstract
{
    public interface ISecuritySettingsService
    {
        string GetEncryptionKey();

        FiledSettingsCollection GetSettings();
    }
}