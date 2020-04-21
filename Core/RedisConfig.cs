using System.Collections.Specialized;
using System.Configuration;

namespace Core
{
    public static class RedisConfig
    {
        private const string SECTION_NAME = "RedisConfig";
        private const string CONNECTION_STRING = "ConnectionString";

        public static string ConnectionString { get; private set; }

        static RedisConfig()
        {
            var conf = (NameValueCollection)ConfigurationManager.GetSection(SECTION_NAME);
            ConnectionString = conf[CONNECTION_STRING];
        }
    }
}
