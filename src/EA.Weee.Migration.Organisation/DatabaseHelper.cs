namespace EA.Weee.Migration.Organisation
{
    using System.Configuration;
    using System.Data.SqlClient;

    public class DatabaseHelper
    {
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString);
        }
    }
}
