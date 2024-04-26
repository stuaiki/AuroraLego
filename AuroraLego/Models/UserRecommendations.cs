using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEXGroup3_7.Models;

public class UserRecommendations
{
    [Key]
    public int recommendation_ID { get; set; }
    [ForeignKey("Customer")]
    public int customer_ID { get; set; }
    public Customer Customer { get; set; }
    public string? rec1 { get; set; }
    public string? rec2 { get; set; }
    public string? rec3 { get; set; }
    public string? rec4 { get; set; }
    public string? rec5 { get; set; }
}