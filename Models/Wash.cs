using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWashOps.Models
{
    public class Wash
    {
        [Key]
        [Column("CarID")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Car name is required.")]
        [StringLength(100)]
        [Display(Name = "Car Name")]
        public string CarName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "License Plate")]
        public string? LicensePlate { get; set; }

        [Required(ErrorMessage = "Wash time is required.")]
        [Display(Name = "Wash Time")]
        [DataType(DataType.Time)]
        public TimeSpan WashTime { get; set; }

        [Display(Name = "Wash Date")]
        [DataType(DataType.Date)]
        public DateTime WashDate { get; set; } = DateTime.Today;

        [StringLength(50)]
        [Display(Name = "Service Type")]
        public string? ServiceType { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "In Progress";

        // Employee responsible for this wash
        [Display(Name = "Employee Responsible")]
        public int? EmployeeNum { get; set; }
        public Employee? Employee { get; set; }

        // Client this car belongs to (optional - walk-ins allowed)
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
