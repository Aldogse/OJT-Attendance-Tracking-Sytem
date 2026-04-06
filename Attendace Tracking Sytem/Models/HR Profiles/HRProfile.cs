using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Attendace_Tracking_Sytem.Models.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Attendace_Tracking_Sytem.Models.HR_Profiles
{
    public class HRProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required(ErrorMessage ="FirstName is Required!")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "LastName is Required!")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;

        [Required(ErrorMessage = "Address is Required!")]
        [StringLength(50)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage="Contact Number is required")]
        [StringLength(11)]
        public string ContactNumber {  get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public LogInCredentials User{ get; set; }
    }
}
