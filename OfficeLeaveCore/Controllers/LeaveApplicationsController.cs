using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeLeaveCore.Data;
using OfficeLeaveCore.Dto;
using OfficeLeaveCore.Models;

namespace OfficeLeaveCore.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/leave-applications")]
        // GET: LeaveApplications
        public async Task<IActionResult> Index()
        {
            //get all applicants
            var leaveApplicants = await _context.LeaveApplications.ToListAsync();

            List<LeaveApplicationDTO> empLeaveApplicants = new List<LeaveApplicationDTO>();
            foreach(var item in leaveApplicants)
            {
                var user = await _context.Users.Where(u => u.Id.Equals(item.UserId)).FirstOrDefaultAsync();
             
                var empApproval = await _context.LeaveApprovals.Where(x => x.ApplicationId.Equals(item.Id)).FirstOrDefaultAsync();
                var approvalFlag = false;
                if(empApproval != null)
                {
                    approvalFlag =  empApproval.ApprovingFlag;
                }

                if (user != null)
                {
                    empLeaveApplicants.Add(new LeaveApplicationDTO()
                    {
                        Id = item.Id,
                        ApplicantName = user.FullName,
                        ApplicantEmail = user.Email,
                        LeaveType = item.LeaveType,
                        Reason = item.Reason,
                        LeaveStart = item.LeaveStart,
                        LeaveEnd = item.LeaveEnd,
                        ApprovalFlag = approvalFlag,
                    });
                }
            }

            ViewBag.CanApprove = await HasAuthToApproveAsync();

            return View(empLeaveApplicants);
        }

        // GET: LeaveApplications/Details/5
        [Route("/leave-applications/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            var user = await _context.Users.Where(u => u.Id.Equals(leaveApplication.UserId)).FirstOrDefaultAsync();

            if (leaveApplication == null || user == null)
            {
                return NotFound();
            }



            ViewBag.ApplicantName = user.FullName;
            ViewBag.ApplicantEmail = user.Email;
           

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Create
        [Route("/leave-applications/create")]
        public async Task<IActionResult> Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            var loggedInUser = await _userManager.GetUserAsync(User);
            ViewBag.FullName = loggedInUser.FullName;
            ViewBag.Email = loggedInUser.Email;
            return View();
        }

        // POST: LeaveApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/leave-applications/create")]
        public async Task<IActionResult> Create([Bind("Id,UserId,LeaveType,Reason,LeaveStart,LeaveEnd,CreatedAt,UpdatedAt")] LeaveApplication leaveApplication)
        {
            if (ModelState.IsValid)
            {
                //generate uuid 
                leaveApplication.Id = Guid.NewGuid().ToString();

                var user = await _userManager.GetUserAsync(User);
                leaveApplication.UserId = user.Id;

                _context.Add(leaveApplication);
                await _context.SaveChangesAsync();

                TempData["SuccessMsg"] = "Leave applied successfully";
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", leaveApplication.UserId);
            TempData["ErrorMsg"] = "Something went wrong.";
            return View(leaveApplication);
        }

        // GET: LeaveApplications/Edit/5
        [Route("/leave-applications/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            var user = await _context.Users.Where(u => u.Id.Equals(leaveApplication.UserId)).FirstOrDefaultAsync();
            leaveApplication.ApplicantName = user.FullName;
            leaveApplication.ApplicantEmail = user.Email;

            if (leaveApplication == null)
            {
                return NotFound();
            }
            return View(leaveApplication);
        }

        // POST: LeaveApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/leave-applications/edit/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserId,LeaveType,Reason,LeaveStart,LeaveEnd")] LeaveApplication leaveApplication)
        {
            //return Json(leaveApplication);
            if (id != leaveApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    _context.Update(leaveApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplicationExists(leaveApplication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMsg"] = "Leave application updated successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMsg"] = "Something went wrong. Try again";
            return View(leaveApplication);
        }

        [HttpDelete]
        [Route("/leave-applications/delete/{id}")]
        public async Task<IActionResult> DeleteLeaveApplication(string id)
        {
            try
            {
                var leaveApplication = await _context.LeaveApplications.FindAsync(id);
                _context.LeaveApplications.Remove(leaveApplication);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                TempData["SuccessMsg"] = "Leave application deleted successfully";
                return Json("Leave Application deleted successfully.");
            }
            catch (Exception ex)
            {

                TempData["ErrorMsg"] = ex.Message.ToString();
                return Json(ex.Message.ToString());
            }
        }

        private bool LeaveApplicationExists(string id)
        {
            return _context.LeaveApplications.Any(e => e.Id == id);
        }

        private async Task<bool> HasAuthToApproveAsync()
        {
            var canApprove = false;
            //get user info
            var user  = await _userManager.GetUserAsync(User);

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if(role.Equals("Member"))
                {
                    canApprove = false;
                    break;
                }
                else
                {
                    canApprove = true;
                }
            }

            return canApprove;
        }
    }
}
