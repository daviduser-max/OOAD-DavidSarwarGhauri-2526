using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DokterspraktijkLib
{
    public static class Database
    {
        private static string connectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=DokterspraktijkDB;Trusted_Connection=True;";

        public static SqlConnection GetConnection() {  return new SqlConnection(connectionString); }
    }
}
