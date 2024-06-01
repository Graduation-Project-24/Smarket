using Smarket.Models.Enum;

namespace Smarket.Models.DTOs
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }


    }
}
