using System.Data.SqlClient;
using System.Configuration;

namespace Converter_DesktopApp_Sql_Database
{
    public class MyConnection
    {
        private readonly SqlConnection connection = GetConnection();
        public static SqlConnection GetConnection()
        {
            return new SqlConnection() { ConnectionString = "server=DESKTOP-DTPF1JL; database=Converter; Integrated Security=true " }; 
        }

        public void InsertCategory(int newCateId, string cateName)
        {
            connection.Open();
            SqlCommand insertCate = new($"insert into dbo.CATEGORIES (CATE_ID,CATE_NAME) values ({newCateId + 1},'{cateName}')", connection);
            _ = insertCate.ExecuteNonQuery();
            connection.Close();

        }
        public void InsertUnit(int newUnitId, string unitName, int newCateId, string value)
        {
            connection.Open();
            SqlCommand insertUnit = new($"insert into dbo.UNITS (UNIT_ID,UNIT_NAME,CATE_ID,VALUE) values ({newUnitId + 1},'{unitName}',{newCateId},'{value}')", connection);
            _ = insertUnit.ExecuteNonQuery();
            connection.Close();

        }
    }
}
