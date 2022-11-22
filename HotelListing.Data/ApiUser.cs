using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Data
{
    public class ApiUser : IdentityUser
    {
        [MaxLength(256)]
        public string FirstName { get; set; }

        [MaxLength(450)]
        public string LastName { get; set; }
    }
}
