using Fabric.App.Utils;
using Fabric.DataAccess.Contracts;
using Fabric.DataAccess.Entities;
using Fabric.DataAccess.Impl;
using Fabric.DataAccess.Utils;
using System.Resources;

namespace Fabric.App
{
    internal class AppConfig
    {
        public static async void GetInfoDB(string msg, List<Product> pd)
        {
            Console.WriteLine($"\n\n{msg}");
            pd = new List<Product>(await ProductRepos.GetAllAsync());

            foreach (var product in pd)
            {
                Console.WriteLine(product.ToString());
            }
        }
        public static ProductsRepository ProductRepos { get; set; }
        public static async Task Initialize()
        {
            ConnectionStringHolder connectionStringHolder = new();
            DataBaseMigrate dataBaseMigrate = new(connectionStringHolder.GetConnectionString());
            ResourceManager resourceManager = new ResourceManager("Fabric.Resources.ApplicationResource", typeof(AppConfig).Assembly);

            Mode modeFromResource = (Mode)Convert.ToInt32(resourceManager.GetString("mode"));

            //if (Mode.DEV.Equals(modeFromResource))
            //    await dataBaseMigrate.Migrate();
            // Не працює, не виправили ніби то ще на тій парі, я щось грався і хз що не так XD
            // На жаль у мене не має можливості вибирати СКБД, тільки зараз побачив що воно через інтерфейс робиться, а я щось консольний дідо
            //---------------
            Product updateProduct = new Product
            {
                ID = 1,
                Name = "Test",
                Type = (TypeProduct)1,
                Color = ColorProduct.Blue,
                Calories = 1
            };
            ProductRepos = new ProductRepositoryImpl(connectionStringHolder.GetConnectionString());
            List<Product> products = new();
            GetInfoDB("FIRST", products);

            await ProductRepos.UpdateAsync(updateProduct);
            GetInfoDB("NEW", products);

            await ProductRepos.DeleteAsync(updateProduct);
            GetInfoDB("DELETE", products);

            await ProductRepos.CreateAsync(updateProduct);
            GetInfoDB("CREATE", products);
        }
    }
}
