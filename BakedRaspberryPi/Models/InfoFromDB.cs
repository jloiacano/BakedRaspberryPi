using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using BakedRaspberryPi.Models;

namespace BakedRaspberryPi.Models
{
    public class InfoFromDB
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BakedPiDB"].ConnectionString;

        public List<string> GetBakedDatabaseInfo(string sqlRequest, object model)
        {
            List<string> response = new List<string>();

            using (SqlConnection thisRequest = new SqlConnection(sqlRequest))
            {
                thisRequest.Open();

                SqlCommand command = thisRequest.CreateCommand();
                command.CommandText = sqlRequest;

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        response.Add(reader.GetString(0));
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