using System.Data.SqlClient;

namespace Converter_DesktopApp_Sql_Database
{
    public class MyConnection
    {
        private readonly string connectionString;

        public MyConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void InsertCategory(int newCateId, string cateName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string query = "INSERT INTO dbo.CATEGORIES (CATE_ID, CATE_NAME) VALUES (@CateId, @CateName)";
                using (var insertCate = new SqlCommand(query, connection))
                {
                    insertCate.Parameters.AddWithValue("@CateId", newCateId + 1);
                    insertCate.Parameters.AddWithValue("@CateName", cateName);

                    _ = insertCate.ExecuteNonQuery();
                }
            }
        }

        public void InsertUnit(int newUnitId, string unitName, int newCateId, string value)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string query = "INSERT INTO dbo.UNITS (UNIT_ID, UNIT_NAME, CATE_ID, VALUE) VALUES (@UnitId, @UnitName, @CateId, @Value)";
                using (var insertUnit = new SqlCommand(query, connection))
                {
                    insertUnit.Parameters.AddWithValue("@UnitId", newUnitId + 1);
                    insertUnit.Parameters.AddWithValue("@UnitName", unitName);
                    insertUnit.Parameters.AddWithValue("@CateId", newCateId);
                    insertUnit.Parameters.AddWithValue("@Value", value);

                    _ = insertUnit.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(string cateName, int cateId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string deleteCateQuery = "DELETE FROM dbo.CATEGORIES WHERE CATE_NAME = @CateName";
                using (var deleteCate = new SqlCommand(deleteCateQuery, connection))
                {
                    deleteCate.Parameters.AddWithValue("@CateName", cateName);
                    _ = deleteCate.ExecuteNonQuery();
                }

                //delete all related Units:
                string deleteUnitQuery = "DELETE FROM dbo.UNITS WHERE CATE_ID = @CateId";
                using (var deleteUnit = new SqlCommand(deleteUnitQuery, connection))
                {
                    deleteUnit.Parameters.AddWithValue("@CateId", cateId);
                    _ = deleteUnit.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUnit(string unitName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string deleteUnitQuery = "DELETE FROM dbo.UNITS WHERE UNIT_NAME = @UnitName";
                using (var deleteUnit = new SqlCommand(deleteUnitQuery, connection))
                {
                    deleteUnit.Parameters.AddWithValue("@UnitName", unitName);
                    _ = deleteUnit.ExecuteNonQuery();
                }
            }
        }

        public void EditValue(string unitName, string value)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection
                string editValueQuery = "UPDATE dbo.UNITS SET VALUE = @Value WHERE UNIT_NAME = @UnitName";
                using (var editValue = new SqlCommand(editValueQuery, connection))
                {
                    editValue.Parameters.AddWithValue("@Value", value);
                    editValue.Parameters.AddWithValue("@UnitName", unitName);
                    _ = editValue.ExecuteNonQuery();
                }
            }
        }
    }
}
