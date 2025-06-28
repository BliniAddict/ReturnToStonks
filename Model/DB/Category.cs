namespace ReturnToStonks
{
    public class Category
    {
        public Category(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public Category(Category category)
        {
            Name = category.Name;
            Symbol = category.Symbol;
        }

        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
