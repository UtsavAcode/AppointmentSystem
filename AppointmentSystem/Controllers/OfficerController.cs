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
        private readonly ILogger<OfficerController> _logger;

        public OfficerController(IOfficerService officerService, IPostService postService,
            ApplicationDbContext context, ILogger<OfficerController> logger)
        {
            _officerService = officerService;
            _postService = postService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var officers = await _officerService.GetAllOfficersWithPostAsync();
                return View(officers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading officers");
                TempData["Error"] = "Error loading officers: " + ex.Message;
                return View(new List<OfficerViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var posts = _postService.GetActivePostAsync().Result;
            ViewBag.Posts = new SelectList(posts, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfficerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Status = true; 
                    model.WorkDays = model.WorkDays ?? new List<WorkDayViewModel>();

                   
                    if (!TimeOnly.TryParse(model.WorkStartTime, out _) ||
                        !TimeOnly.TryParse(model.WorkEndTime, out _))
                    {
                        ModelState.AddModelError("", "Invalid time format. Please use HH:mm format.");
                        var posts = await _postService.GetActivePostAsync();
                        ViewBag.Posts = new SelectList(posts, "Id", "Name");
                        return View(model);
                    }

                    await _officerService.CreateOfficerAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating officer");
                    ModelState.AddModelError("", "Error creating officer: " + ex.Message);
                }
            }

            var postsForError = await _postService.GetActivePostAsync();
            ViewBag.Posts = new SelectList(postsForError, "Id", "Name");
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

                var posts = await _postService.GetActivePostAsync();
                ViewBag.Posts = new SelectList(posts, "Id", "Name");
                return View(officer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for officer {OfficerId}", id);
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
                try
                {
                    
                    if (!TimeOnly.TryParse(model.WorkStartTime, out _) ||
                        !TimeOnly.TryParse(model.WorkEndTime, out _))
                    {
                        ModelState.AddModelError("", "Invalid time format. Please use HH:mm format.");
                        var posts = await _postService.GetActivePostAsync();
                        ViewBag.Posts = new SelectList(posts, "Id", "Name");
                        return View(model);
                    }

                  
                    if (model.WorkDays != null)
                    {
                        foreach (var workDay in model.WorkDays)
                        {
                            if (workDay.DayOfWeek < 1 || workDay.DayOfWeek > 7)
                            {
                                ModelState.AddModelError("", $"Invalid day of week: {workDay.DayOfWeek}. Must be between 1 and 7.");
                                var posts = await _postService.GetActivePostAsync();
                                ViewBag.Posts = new SelectList(posts, "Id", "Name");
                                return View(model);
                            }
                        }
                    }

                    await _officerService.UpdateOfficerAsync(model);
                    TempData["Success"] = "Officer updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "Invalid time format while updating officer {OfficerId}", model.Id);
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating officer {OfficerId}", model.Id);
                    ModelState.AddModelError("", "Error updating officer: " + ex.Message);
                }
            }

            var postsForError = await _postService.GetActivePostAsync();
            ViewBag.Posts = new SelectList(postsForError, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool status)
        {
            try
            {
                if (status)
                {
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
                _logger.LogError(ex, "Error toggling status for officer {OfficerId}", id);
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