using System.ComponentModel.DataAnnotations;

namespace INTEXGroup3_7.Models;

public class OrderPrediction
{
    [Key]
    public int predictionId { get; set; }
    public Order order { get; set; }
    public string prediction { get; set; }
}