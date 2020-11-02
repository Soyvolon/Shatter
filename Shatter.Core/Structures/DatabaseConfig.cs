
using Newtonsoft.Json;

using Npgsql;

namespace Shatter.Core.Structures
{
    public class DatabaseConfig
    {
        [JsonProperty("GeneralDb")]
        public string GeneralDb { get; private set; }

        [JsonProperty("database")]
        private string database;
        [JsonProperty("hostname")]
        private string hostname;
        [JsonProperty("username")]
        private string username;
        [JsonProperty("password")]
        private string password;
        [JsonProperty("port")]
        private int port;

        [JsonConstructor]
        public DatabaseConfig(string database, string hostname, string username, string password, int port, string GeneralDb)
        {
            this.database = database;
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.GeneralDb = GeneralDb;
            this.port = port;
        }

        // Use this for connecting to a database.
        public string GetConnectionString()
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = hostname,
                Username = username,
                Password = password,
                Database = database,
                Port = port
            }.ConnectionString;
        }
    }
}
