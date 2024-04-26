using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEXGroup3_7.Models;

public class ItemRecommendation
{
    [Key]
    public int item_rec_ID { get; set; }
    [ForeignKey("Product")]
    public int product_ID { get; set; }
    public int? rec1 { get; set; }
    public int? rec2 { get; set; }
    public int? rec3 { get; set; }
    public int? rec4 { get; set; }
    public int? rec5 { get; set; }
    public int? rec6 { get; set; }
    public int? rec7 { get; set; }
    public int? rec8 { get; set; }
    public int? rec9 { get; set; }
    public int? rec10 { get; set; }
}