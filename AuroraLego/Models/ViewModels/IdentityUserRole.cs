using Microsoft.AspNetCore.Identity;

namespace INTEXGroup3_7.Models.ViewModels
{
    public class IdentityUserRole
    {
        public IdentityUser User { get; set; }
        public IdentityRole Role { get; set; }
    }
}