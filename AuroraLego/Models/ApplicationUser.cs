using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace INTEXGroup3_7.Models;

public class ApplicationUser : IdentityUser
{
  [Required]
  public string Id { get; set; }
  
  [Required]
  public string Email { get; set; }
  
  [ForeignKey("Customer")]
  public int customer_ID { get; set; } // Foreign key

  public Customer Customer { get; set; } // Navigation property
}