using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEXGroup3_7.Models;

public class top_product
{
    [Key]
    public int top_product_ID { get; set; }
    [ForeignKey("Product")]
    public int product_ID { get; set; }
    public Product Product { get; set; }
    public float? average_rating { get; set; }
    public int? ratings_count { get; set; }
    public float? popularity_score { get; set; }
}