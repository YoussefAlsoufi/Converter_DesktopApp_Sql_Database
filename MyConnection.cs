using System.Collections.Generic;
using System.Data.SqlClient;

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
        public void DeleteCategory(string cateName, int cateId)
        {
            //delete the Cate:
            connection.Open();
            SqlCommand deleteCate = new($"delete from dbo.CATEGORIES where CATE_NAME = '{cateName}';", connection);
            _ = deleteCate.ExecuteNonQuery();
            connection.Close();
            //delete all related Units:
            connection.Open();
            SqlCommand deleteUnit = new($"delete from dbo.UNITS where CATE_ID = {cateId};", connection);
            _ = deleteUnit.ExecuteNonQuery();
            connection.Close();

        }
        public void DeleteUnit(string unitName)
        {
            connection.Open();
            SqlCommand deleteUnit = new($"delete from dbo.UNITS where UNIT_NAME = '{unitName}';", connection);
            _ = deleteUnit.ExecuteNonQuery();
            connection.Close();
        }
        public void EditValue(string unitName, int cateId)
        {
            connection.Open();
            SqlCommand editValue = new($"UPDATE dbo.UNITS SET VALUE = 1000 WHERE UNIT_NAME = '{unitName}' AND CATE_ID = {cateId}; ", connection);
            _ = editValue.ExecuteNonQuery();
            connection.Close();
        }
            
    }
}
