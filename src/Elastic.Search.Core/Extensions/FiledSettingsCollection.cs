using System.Collections.Generic;
using System.Linq;
using Elastic.Search.Core.Models;

namespace Elastic.Search.Core.Extensions
{
    public class FiledSettingsCollection
    {
        private readonly IList<FiledSettings> _settings;

        public FiledSettingsCollection(IList<FiledSettings> settings)
        {
            _settings = settings;
        }

        public bool IsExactMatch(string name)
        {
            var item = _settings.FirstOrDefault(s => s.Name == name);
            return item == null || item.ExactMatch;
        }

        public bool IsCrypted(string name)
        {
            var item = _settings.FirstOrDefault(s => s.Name == name);
            return item != null && item.Crypted;
        }

        public bool AllowSort(string name)
        {
            var item = _settings.FirstOrDefault(s => s.Name == name);
            return item == null || item.Sortable;
        }
    }
}