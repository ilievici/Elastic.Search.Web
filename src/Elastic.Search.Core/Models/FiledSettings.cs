namespace Elastic.Search.Core.Models
{
    public class FiledSettings
    {
        public string Name { get; }

        public bool Crypted { get; }

        public bool ExactMatch { get; }

        public bool Sortable { get; }

        public bool OutputField { get; }

        public FiledSettings(string fieldName, bool crypted, bool exactMatch, bool sortable = true, bool outputField = true)
        {
            Name = fieldName;
            Crypted = crypted;
            ExactMatch = exactMatch;
            Sortable = sortable;
            OutputField = outputField;
        }
    }
}