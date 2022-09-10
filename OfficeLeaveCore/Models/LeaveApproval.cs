using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Models
{
    public class LeaveApproval
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("LeaveApplication")]
        [Required]
        public string ApplicationId { get; set; }
        public LeaveApplication LeaveApplication { get; set; }


        [Required]
        public bool ApprovingFlag { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApprovingOfficerId { get; set; }
        public ApplicationUser AprrovingOfficer { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
