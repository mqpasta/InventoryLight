using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace TestCore.Models.SqlRepository
{
    public class DBHelper
    {
        private static SqlConnection _connection;
        private static string _connectionString = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3};{4}";

        public static void SetConnectionString(string dbServer, string dbName,
                                                string userId, string password,
                                                bool isLocal)
        {
            _connectionString = string.Format(_connectionString, @dbServer, @dbName,
                                                userId, password,
                                                isLocal ? "Trusted_Connection=True" : "");
            Console.WriteLine(_connectionString);
        }

        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        private DBHelper()
        {

        }

        public static SqlConnection Instance
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_connectionString);
                }
                return _connection;
            }
        }

        public static DataSet LoadData(SqlConnection con, string query)
        {
            DataSet ds = new DataSet();// to store reuslt

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds);

            return ds;
        }

        public static object ExecuteScalar(SqlConnection con, string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);
            return cmd.ExecuteScalar();
        }

        public static object ExecuteScalar(SqlConnection con, string query, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand(query, con, trans);
            return cmd.ExecuteScalar();
        }

        public static void Execute(SqlConnection con, string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
        }

        public static void Execute(SqlConnection con, string query, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand(query, con, trans);
            cmd.ExecuteNonQuery();
        }

        public static string NullOrValue<T>(Nullable<T> value) where T : struct
        {
            string res = "NULL";

            if (value != null)
            {
                if (value is Nullable<bool>)
                {
                    Nullable<bool> t = (Nullable<bool>)(object)value;
                    if (t.Value)
                        return "1";
                }
                else if(value is Nullable<DateTime>)
                {
                    return "'" + value.ToString() + "'";
                }

                return value.ToString();
            }


            return res;
        }
    }
}
