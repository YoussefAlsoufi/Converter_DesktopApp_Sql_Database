using System.Data.SqlClient;
using System.Configuration;

namespace Converter_DesktopApp_Sql_Database
{
    public static class MyConnection
    {
        public static SqlConnection GetConnection()
        {
            return new SqlConnection() { ConnectionString = "server=DESKTOP-DTPF1JL; database=Converter; Integrated Security=true " }; 
        }
    }
}
