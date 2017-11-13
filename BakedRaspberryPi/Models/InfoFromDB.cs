using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using BakedRaspberryPi.Models;
using MySql.Data.MySqlClient;

namespace BakedRaspberryPi.Models
{
    public class InfoFromDB
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BakedPiDB"].ConnectionString;

        public List<T> GetBakedDatabaseInfo<T>(string sqlRequest)
        {
            List<T> response = new List<T>();
            
            using (MySqlConnection thisRequest = new MySqlConnection(connectionString))
            {
                thisRequest.Open();

                MySqlCommand command = thisRequest.CreateCommand();
                command.CommandText = sqlRequest;

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string t = reader.GetString(0);
                       
                    }
                }

                thisRequest.Close();
            }
            return response;
        }

        public int SetBakedDatabaseInfo(string sqlStatement)
        {
            int setResponse = 0;

            using (SqlConnection thisRequest = new SqlConnection(connectionString))
            {
                thisRequest.Open();

                SqlCommand command = new SqlCommand(sqlStatement, thisRequest);
                command.Connection.Open();
                setResponse = command.ExecuteNonQuery();
                if (thisRequest.State == System.Data.ConnectionState.Open)
                {
                    thisRequest.Close();
                }
                return setResponse;
            }
        }
    }
}