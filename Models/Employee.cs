using System.ComponentModel.DataAnnotations;

namespace CarWashOps.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeNum { get; set; }

        [Required(ErrorMessage = "Employee name is required.")]
        [StringLength(100)]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service type is required.")]
        [StringLength(50)]
        [Display(Name = "Service Type")]
        public string ServiceType { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation: washes this employee has handled
        public ICollection<Wash>? Washes { get; set; }
    }
}
