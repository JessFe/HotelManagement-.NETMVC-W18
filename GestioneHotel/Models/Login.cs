using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class Login
    {
        [Required]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}