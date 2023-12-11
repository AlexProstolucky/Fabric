using System.Data.Common;
using System.Data.SQLite;

namespace Fabric.DataAccess.Utils
{
    internal class DataBaseMigrate
    {
        private readonly string sqlFilePath = Path.Combine("Resources", "MOCK_DATA.sql");
        private readonly string sqliteFile = Path.Combine("Data", "Products.sqlite");
        private string ConnectionString { get; init; }
        private SQLiteFactory SQLiteFactory { get; init; }

        public DataBaseMigrate(string connectionString)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SQLite.SQLiteFactory.Instance);
            SQLiteFactory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SqlClient");
            ConnectionString = connectionString;
        }

        internal async Task Migrate()
        {
            string sqlCommands = File.ReadAllText(sqlFilePath);

            if (!File.Exists(sqliteFile))
            {
                Directory.CreateDirectory("Data");
                File.Create(sqliteFile);
            }

            using DbConnection connection = SQLiteFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();

            using DbCommand command = SQLiteFactory.CreateCommand();
            command.Connection = connection;
            string[] separators = new string[] { ";\r\n", ";\n", ";\r" };
            string[] commands = sqlCommands.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            DbTransaction transaction = await connection.BeginTransactionAsync();
            foreach (string commandText in commands)
            {
                command.CommandText = commandText;
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        }
    }
}
