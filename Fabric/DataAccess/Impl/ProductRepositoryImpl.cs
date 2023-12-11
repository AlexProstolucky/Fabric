using Fabric.DataAccess.Contracts;
using Fabric.DataAccess.Entities;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace Fabric.DataAccess.Impl
{
    internal class ProductRepositoryImpl : ProductsRepository
    {
        private const string SQL_FIND_GET_ALL = "SELECT * FROM Products";
        public string ConnectionString { get; init; }
        private SQLiteFactory SqliteFactory { get; init; }
        public ProductRepositoryImpl(string connectionString)
        {
            ConnectionString = connectionString;
            SqliteFactory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        public async Task<IEnumerable<Product?>> GetAllAsync()
        {
            List<Product?> products = new List<Product?>();
            using DbConnection connection = SqliteFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();

            using DbCommand command = connection.CreateCommand();
            command.CommandText = SQL_FIND_GET_ALL;
            Stopwatch stopwatch = Stopwatch.StartNew();
            using DbDataReader reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                Product product = new Product
                {
                    ID = reader.GetInt32("ID"),
                    Name = reader.GetString("Name"),
                    Type = (TypeProduct)reader.GetInt32("Type"),
                    Color = (ColorProduct)reader.GetInt32("Color"),
                    Calories = reader.GetInt32("Calories")
                };

                products.Add(product);
            }
            stopwatch.Stop();
            Console.WriteLine($"\nQuery execution time: {stopwatch.ElapsedMilliseconds} milliseconds\n");
            return products;
        }
        public async Task CreateAsync(Product product)
        {
            using DbConnection connection = SqliteFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();

            using DbCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Products (Name, Type, Color, Calories) VALUES (@Name, @Type, @Color, @Calories)";

            DbParameter nameParameter = command.CreateParameter();
            nameParameter.ParameterName = "@Name";
            nameParameter.Value = product.Name;
            command.Parameters.Add(nameParameter);

            DbParameter typeParameter = command.CreateParameter();
            typeParameter.ParameterName = "@Type";
            typeParameter.Value = product.Type;
            command.Parameters.Add(typeParameter);

            DbParameter colorParameter = command.CreateParameter();
            colorParameter.ParameterName = "@Color";
            colorParameter.Value = product.Color;
            command.Parameters.Add(colorParameter);

            DbParameter caloriesParameter = command.CreateParameter();
            caloriesParameter.ParameterName = "@Calories";
            caloriesParameter.Value = product.Calories;
            command.Parameters.Add(caloriesParameter);
            Stopwatch stopwatch = Stopwatch.StartNew();
            await command.ExecuteNonQueryAsync();

            stopwatch.Stop();
            Console.WriteLine($"\nQuery execution time: {stopwatch.ElapsedMilliseconds} milliseconds\n");

        }

        public async Task UpdateAsync(Product product)
        {
            using DbConnection connection = SqliteFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();

            DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE ID = @ID";
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@ID";
            parameter.Value = product.ID;

            command.Parameters.Add(parameter);
            Stopwatch stopwatch = Stopwatch.StartNew();
            using DbDataReader reader = await command.ExecuteReaderAsync();

            if (reader.Read())
            {
                using DbCommand upcomand = connection.CreateCommand();
                upcomand.CommandText = "UPDATE Products SET Name = @Name, Type = @Type, Color = @Color, Calories = @Calories WHERE ID = @Id";
                DbParameter nameParameter = upcomand.CreateParameter();
                nameParameter.ParameterName = "@Name";
                nameParameter.Value = product.Name;
                upcomand.Parameters.Add(nameParameter);

                DbParameter typeParameter = upcomand.CreateParameter();
                typeParameter.ParameterName = "@Type";
                typeParameter.Value = product.Type;
                upcomand.Parameters.Add(typeParameter);

                DbParameter colorParameter = upcomand.CreateParameter();
                colorParameter.ParameterName = "@Color";
                colorParameter.Value = product.Color;
                upcomand.Parameters.Add(colorParameter);

                DbParameter caloriesParameter = upcomand.CreateParameter();
                caloriesParameter.ParameterName = "@Calories";
                caloriesParameter.Value = product.Calories;
                upcomand.Parameters.Add(caloriesParameter);

                DbParameter idParameter = upcomand.CreateParameter();
                idParameter.ParameterName = "@Id";
                idParameter.Value = product.ID;
                upcomand.Parameters.Add(idParameter);

                upcomand.ExecuteNonQuery();

            }
            stopwatch.Stop();
            Console.WriteLine($"\nQuery execution time: {stopwatch.ElapsedMilliseconds} milliseconds\n");
        }
        public async Task DeleteAsync(Product product)
        {
            using DbConnection connection = SqliteFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();

            DbCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE ID = @ID";
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@ID";
            parameter.Value = product.ID;

            command.Parameters.Add(parameter);
            Stopwatch stopwatch = Stopwatch.StartNew();
            await command.ExecuteNonQueryAsync();
            stopwatch.Stop();
            Console.WriteLine($"\nQuery execution time: {stopwatch.ElapsedMilliseconds} milliseconds\n");
        }
    }
}
