using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SL_App
{
    public class SqlManager
    {
        private string _connectionString = "";

        public SqlManager()
        {
            IsConnected = false;
        }

        public void Connect(string connectionId)
        {
            if(connectionId == null)
            {
                Console.WriteLine("ConnecctionId cannot be null!");
                return;
            }

            string json = File.ReadAllText("connections.json");
            var connections = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            bool success = connections.TryGetValue(connectionId, out string connectionString);

            if (!success)
            {
                Console.WriteLine("Could not find a connection for id '{0}'!", connectionId);
                return;
            }

            _connectionString = connectionString;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    IsConnected = true;
                    Console.WriteLine("Connected!");
                }
                catch (InvalidOperationException)
                {
                    IsConnected = false;
                }
                catch (SqlException)
                {
                    IsConnected = false;
                }
            }
        }

        public bool IsConnected { get; private set; }
    }
}
