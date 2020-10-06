
using Newtonsoft.Json;

using Npgsql;

namespace NitroSharp.Structures
{
    public class DatabaseConfig
    {
        [JsonProperty]
        public string GeneralDb { get; private set; }

        [JsonProperty]
        private string database;
        [JsonProperty]
        private string hostname;
        [JsonProperty]
        private string username;
        [JsonProperty]
        private string password;
        [JsonProperty]
        private int port;

        [JsonConstructor]
        public DatabaseConfig(string db, string host, string user, string pass, string generalName, int port)
        {
            database = db;
            hostname = host;
            username = user;
            password = pass;
            GeneralDb = generalName;
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
