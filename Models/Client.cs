using System.ComponentModel.DataAnnotations;

namespace CarWashOps.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(100)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        [Display(Name = "License Plate")]
        public string? LicensePlate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation: this client's wash history
        public ICollection<Wash>? Washes { get; set; }
    }
}
