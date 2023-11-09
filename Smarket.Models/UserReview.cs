using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smarket.Models
{
    public class UserReview
    {
        public int Id { get; set; }
        public double Rate { get; set; }
        public string Comment { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
