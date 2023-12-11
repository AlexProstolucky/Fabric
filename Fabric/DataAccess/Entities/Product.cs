namespace Fabric.DataAccess.Entities
{
    internal class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TypeProduct Type { get; set; }
        public ColorProduct Color { get; set; }

        public int Calories { get; set; }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return $"{ID} | {Name} | {Type.ToString()} | {Color.ToString()} | {Calories.ToString()}";
        }
    }
}
