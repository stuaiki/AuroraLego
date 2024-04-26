using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace INTEXGroup3_7.Models
{
    public class Customer
    {
        [Key]
        public int customer_ID { get; set; }
        
        public string first_name { get; set; }

        public string last_name { get; set; }

        public DateOnly birth_date { get; set; }

        public string country_of_residence { get; set; }

        public string gender { get; set; }

        public int age { get; set; }
    }
}
