using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEXGroup3_7.Models
{
    public class LineItem
    {
        [Key]
        public int line_ID { get; set; }

        [ForeignKey("Order")]
        public int transaction_ID { get; set; }
        public Order Order { get; set; }

        [ForeignKey("Product")]
        public int product_ID { get; set; }
        public Product Product { get; set; }

        public int qty { get; set; }

        [Range(1, 5)]
        public int rating { get; set; }
    }
}
