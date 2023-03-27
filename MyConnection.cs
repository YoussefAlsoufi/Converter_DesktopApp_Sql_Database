using System.Data.SqlClient;
using System.Configuration;

namespace Converter_DesktopApp_Sql_Database
{
    public static class MyConnection
    {
        public static SqlConnection GetConnection()
        {
            string con = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection connection = new(con);
            return connection;
        }


        public static SqlConnection TestGetConnection()
        {
            return new SqlConnection() { ConnectionString = "server=DESKTOP-DTPF1JL; database=Converter; Integrated Security=true " }; 
        }
    }
}
