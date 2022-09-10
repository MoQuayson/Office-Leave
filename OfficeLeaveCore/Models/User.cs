using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
