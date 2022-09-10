using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeLeaveCore.Data;
using OfficeLeaveCore.Models;

namespace OfficeLeaveCore.Controllers
{
    public class LeaveApprovalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveApprovalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LeaveApprovals
        [Route("/leave-applications/{leaveId}/leave-approvals")]
        public async Task<IActionResult> Index(string leaveId)
        {
            //if application doesnt exists
            var idExists = _context.LeaveApplications.Where(x => x.Id.Equals(leaveId)).Any();
            if (!idExists)
            {
                return NotFound();
            }

            //if application has already been reviewed
            var approvalExists = _context.LeaveApprovals.Where(x => x.ApplicationId.Equals(leaveId)).Any();
            if (approvalExists == true)
            {
                var approvalId = _context.LeaveApprovals.Where(x => x.ApplicationId.Equals(leaveId)).Select(x => x.Id).FirstOrDefault();

                var routeValues = new Dictionary<string, string>();
                routeValues.Add("leaveId", leaveId);
                routeValues.Add("id", approvalId);

                return RedirectToAction(actionName: "Edit", controllerName: "LeaveApprovals", routeValues);
            }

            //get leave application
            var leaveApplication = await _context.LeaveApplications.Where(x => x.Id.Equals(leaveId)).FirstOrDefaultAsync();

            //get user info
            var user = _context.Users.Where(u => u.Id.Equals(leaveApplication.UserId)).FirstOrDefault();

            //assign user selected infos
            leaveApplication.ApplicantName = user.FullName;
            leaveApplication.ApplicantEmail = user.Email;


            ViewBag.LeaveApplication = leaveApplication;
            return View();
        }

        // GET: LeaveApprovals/Details/5

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApproval = await _context.LeaveApprovals
                .Include(l => l.LeaveApplication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApproval == null)
            {
                return NotFound();
            }

            return View(leaveApproval);
        }

        // POST: LeaveApprovals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationId,ApprovingFlag,CreatedAt,UpdatedAt")] LeaveApproval leaveApproval)
        {

            if (ModelState.IsValid)
            {

                //generate uuid 
                leaveApproval.Id = Guid.NewGuid().ToString();
                leaveApproval.ApprovingOfficerId = _userManager.GetUserId(User);

                _context.Add(leaveApproval);
                await _context.SaveChangesAsync();

                TempData["SuccessMsg"] = "Leave Application reviewed";
                return RedirectToAction(actionName: "Index", controllerName: "LeaveApplications");
            }

            TempData["ErrorMsg"] = "Something went wrong";
            return RedirectToAction(actionName: "Index", controllerName: "LeaveApplications");
        }

        // GET: LeaveApprovals/Edit/5
        [Route("/leave-applications/{leaveId}/leave-approvals/edit/{id}")]
        public async Task<IActionResult> Edit(string leaveId, string id)
        {
            if (id == null || leaveId == null)
            {
                return NotFound();
            }

            //get leave application
            var leaveApplication = await _context.LeaveApplications.Where(x => x.Id.Equals(leaveId)).FirstOrDefaultAsync();

            //get user info
            var user = _context.Users.Where(u => u.Id.Equals(leaveApplication.UserId)).FirstOrDefault();
            var approvalFlag = _context.LeaveApprovals.Where(x => x.Id.Equals(id)).Select(x => x.ApprovingFlag).FirstOrDefault();
            var leaveApproval = _context.LeaveApprovals.Where(x => x.Id.Equals(id)).FirstOrDefault();
            //assign user selected infos
            leaveApplication.ApplicantName = user.FullName;
            leaveApplication.ApplicantEmail = user.Email;

            
            ViewBag.LeaveApplication = leaveApplication;
            ViewBag.ApprovalFlag = approvalFlag;

            return View(leaveApproval);
        }

        // POST: LeaveApprovals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/leave-applications/{leaveId}/leave-approvals/edit/{id}")]
        public async Task<IActionResult> Edit(string leaveId,string id, [Bind("Id,ApplicationId,ApprovingFlag,CreatedAt,UpdatedAt")] LeaveApproval leaveApproval)
        {
            if (id != leaveApproval.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    leaveApproval.ApprovingOfficerId = _userManager.GetUserId(User);
                    _context.Update(leaveApproval);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApprovalExists(leaveApproval.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMsg"] = "Leave Application reviewed";
                return RedirectToAction(controllerName: "LeaveApplications", actionName: "Index");
            }
            TempData["ErrorMsg"] = "Something went wrong";
            return RedirectToAction(actionName: "Index", controllerName: "LeaveApplications");
        }

        // GET: LeaveApprovals/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApproval = await _context.LeaveApprovals
                .Include(l => l.LeaveApplication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApproval == null)
            {
                return NotFound();
            }

            return View(leaveApproval);
        }

        // POST: LeaveApprovals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var leaveApproval = await _context.LeaveApprovals.FindAsync(id);
            _context.LeaveApprovals.Remove(leaveApproval);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveApprovalExists(string id)
        {
            return _context.LeaveApprovals.Any(e => e.Id == id);
        }
    }
}
