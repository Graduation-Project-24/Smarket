namespace Smarket.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Package> Packages { get; set; }

    }
}
