using System.ComponentModel.DataAnnotations;

namespace Pustok.MVC.Areas.Admin.ViewModels
{
    public class AdminLoginVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsPersistent { get; set; }
    }
}
