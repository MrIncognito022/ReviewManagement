using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Customer

    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Checkout Date")]
        [DataType(DataType.Date)]
        public DateTime CheckoutDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Today's Date")]
        [DataType(DataType.Date)]
        public DateTime TodayDate { get; set; }
    }
}
