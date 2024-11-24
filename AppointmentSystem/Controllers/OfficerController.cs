using AppointmentSystem.Data;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Controllers
{
    public class OfficerController : Controller
    {
        private readonly IOfficerService _officerService;
        private readonly IPostService _postService;
        private readonly ApplicationDbContext _context;

        public OfficerController(IOfficerService officerService, IPostService postService, ApplicationDbContext context)
        {
            _officerService = officerService;
            _postService = postService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var officers = await _officerService.GetAllOfficersAsync();
                return View(officers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading officers: " + ex.Message;
                return View(new List<OfficerViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Populate the dropdown list for posts (example)
            var posts = _postService.GetActivePostAsync().Result; // You can fetch posts asynchronously here
            ViewBag.Posts = new SelectList(posts, "Id", "Name");
            return View();
        }

        // POST: Officer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfficerViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Status = true;
                await _officerService.CreateOfficerAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var officer = await _officerService.GetOfficerByIdAsync(id);
                if (officer == null)
                {
                    TempData["Error"] = "Officer not found.";
                    return RedirectToAction(nameof(Index));
                }

                var posts = _postService.GetActivePostAsync().Result; // You can fetch posts asynchronously here
                ViewBag.Posts = new SelectList(posts, "Id", "Name");
                return View(officer);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading edit form: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OfficerViewModel model)
        {

            if (ModelState.IsValid)
            {
                await _officerService.UpdateOfficerAsync(model);
                return RedirectToAction(nameof(Index)); // Redirect to Index after successful update
            }

            // Model is not valid, return the edit view with errors
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool status)
        {
            try
            {
                if (status)
                {
                    // Check if the associated post is active before activating the officer
                    var officer = await _officerService.GetOfficerByIdAsync(id);
                    if (officer != null)
                    {
                        var post = await _postService.GetPostByIdAsync(officer.PostId);
                        if (post != null && !post.Status)
                        {
                            return BadRequest("Cannot activate officer when associated post is inactive.");
                        }
                    }
                }

                var result = await _officerService.ToggleOfficerStatusAsync(id, status);
                if (result)
                {
                    return Ok();
                }
                return BadRequest("Failed to update status.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the status: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult CheckPostStatus(int postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return Json(new { Status = false, message = "Post not found." });
            }

            return Json(new { Status = post.Status });
        }

    }
}
