using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Dto
{
    public class LeaveApplicationDTO
    {
        public string Id { get; set; }

        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }

        public DateTime LeaveStart { get; set; }

        public DateTime LeaveEnd { get; set; }

        public bool ApprovalFlag { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
