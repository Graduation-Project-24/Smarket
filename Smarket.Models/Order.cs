using Smarket.Models.Enum;

namespace Smarket.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int SessionId { get; set; }
        public double TotalPrice { get; set; }
        public Status Status { get; set; }
    }
}
