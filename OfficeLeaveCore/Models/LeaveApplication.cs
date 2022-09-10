using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Models
{
    public class LeaveApplication
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [NotMapped]
        public string ApplicantName { get; set; }

        [NotMapped]
        public string ApplicantEmail { get; set; }

        [Required]
        [Display(Name ="Leave Type")]
        [StringLength(100)]
        public string LeaveType { get; set; }

        [Required]
        [Display(Name = "Reason")]
        [StringLength(255)]
        public string Reason { get; set; }

        [Required]
        [Display(Name ="Leave From")]
        [DataType(DataType.DateTime)]
        public DateTime LeaveStart { get; set; }

        [Required]
        [Display(Name = "Leave To")]
        [DataType(DataType.DateTime)]
        public DateTime LeaveEnd { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
