
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public class ReviewViewModel
    {
        [Range(3, 4, ErrorMessage = "Invalid number")]
        [Required]
        public int? StatusId { get; set; }
        [Required]
        public string Nmrid { get; set; }
        public int Qnrid { get; set; }
        public string message { get; set; }

    }
}
