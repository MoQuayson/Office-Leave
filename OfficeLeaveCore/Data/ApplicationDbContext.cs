using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using OfficeLeaveCore.Models;
using OfficeLeaveCore.Dto;

namespace OfficeLeaveCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<LeaveApproval> LeaveApprovals { get; set; }
   
    }
}
