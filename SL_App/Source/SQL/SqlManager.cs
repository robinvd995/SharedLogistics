﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.SQL
{
    public class SqlManager
    {
        private string _connectionString = "";

        public SqlManager()
        {
            IsConnected = false;
        }

        public IEnumerable<string> Connect(string connectionId)
        {
            List<string> tables = new List<string>();

            if (connectionId == null)
            {
                Console.WriteLine("ConnecctionId cannot be null!");
                return tables;
            }

            string json = File.ReadAllText(App.SETTINGS_DIR + "connections.json");
            var connections = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            bool success = connections.TryGetValue(connectionId, out string connectionString);

            if (!success)
            {
                Console.WriteLine("Could not find a connection for id '{0}'!", connectionId);
                return tables;
            }

            _connectionString = connectionString;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    tables = GetTables(connectionId, connection);

                    IsConnected = true;
                    Console.WriteLine("Connected!");
                }
                catch (InvalidOperationException)
                {
                    IsConnected = false;
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    IsConnected = false;
                }
            }

            return tables;
        }

        public int ExecuteWithoutResult(string querry)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = CreateCommand(querry, connection))
            {
                connection.Open();
                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
        }

        public ISqlResultSet ExecuteQuerry(string querry)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = CreateCommand(querry, connection))
            {
                connection.Open();

                cmd.Prepare();
                SqlDataReader reader = cmd.ExecuteReader();

                SqlResultBuilder builder = SqlResultBuilder.NewInstance(reader.FieldCount);

                //HashSet<string> set = new HashSet<string>();

                for(int i = 0; i < reader.FieldCount; i++)
                {
                    string typename = reader.GetDataTypeName(i);
                    //set.Add(typename);
                    builder.SetColumnType(i, reader.GetDataTypeName(i));
                }

                /*foreach(string s in set)
                {
                    Console.WriteLine(s);
                }*/

                for(int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    builder.SetColumnName(i, name);
                }

                while (reader.Read())
                {
                    builder.PushRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object val = reader.GetValue(i);
                        builder.AddValue(val);
                    }
                    builder.PopRow();
                }

                return builder.Build();
            }
        }

        public ISqlResultSet ExecuteParameterizedQuerry(string querry, string[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                string sequence = "{" + i + "}";
                querry = querry.Replace(sequence, parameters[i]);
            }

            return ExecuteQuerry(querry);
        }

        public ISqlResultSet ExecuteQuerryFromFile(string filePath)
        {
            string querry = File.ReadAllText(filePath);
            return ExecuteQuerry(querry);
        }

        public ISqlResultSet ExectuteParameterizedQuerryFromFile(string filePath, string[] parameters)
        {
            string querry = File.ReadAllText(filePath);
            return ExecuteParameterizedQuerry(querry, parameters);
        }

        public SqlCommand CreateCommandFromFile(string file, SqlConnection connection)
        {
            string querry = File.ReadAllText(file);
            return CreateCommand(querry, connection);
        }

        public SqlCommand CreateCommand(string querry, SqlConnection connection)
        {
            return new SqlCommand(querry, connection);
        }

        private List<string> GetTables(string connectionId, SqlConnection connection)
        {
            List<string> tables = null;

            using (SqlCommand cmd = CreateCommandFromFile("Sql/GetAllTables.sql", connection))
            {
                cmd.Prepare();
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> dbTables = GetTablesFromResult(reader);
                reader.Close();
                List<string> jsonTables = GetTablesFromJson(connectionId);
                tables = CompareLists(dbTables, jsonTables);
            }

            return tables;
        }

        private List<string> GetTablesFromJson(string connectionId)
        {
            string json = File.ReadAllText(App.SETTINGS_DIR + "tables.json");
            var tableDic = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
            bool success = tableDic.TryGetValue(connectionId, out string[] tableData);

            List<string> tables = new List<string>();
            if (success)
            {
                foreach (string s in tableData)
                {
                    tables.Add(s);
                }
            }

            return tables;
        }

        private List<string> GetTablesFromResult(SqlDataReader reader)
        {
            List<string> dbTables = new List<string>();

            while (reader.Read())
            {
                string value = (string)reader.GetValue(0);
                dbTables.Add(value);
            }

            return dbTables;
        }

        private List<string> CompareLists(List<string> list0, List<string> list1)
        {
            List<string> newlist = new List<string>();

            foreach (string s in list0)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (s.Equals(list1[i]))
                    {
                        newlist.Add(s);
                        list1.RemoveAt(i);
                        break;
                    }
                }
            }

            return newlist;
        }

        public bool IsConnected { get; private set; }
    }
}
