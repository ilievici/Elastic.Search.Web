namespace Elastic.Search.Core.Infrastructure
{
    public class ElasticConnectionOption
    {
        public ElasticConnectionOption()
        {
        }

        public ElasticConnectionOption(string scheme, string host, int port)
        {
            Scheme = scheme;
            Host = host;
            Port = port;
        }

        public string Scheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}