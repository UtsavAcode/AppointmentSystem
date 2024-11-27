using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Implementation;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly IOfficerService _officerService;

        public ActivityController(IActivityService activityService, IOfficerService officerService)
        {
            _activityService = activityService;
            _officerService = officerService;
        }
        public async Task<IActionResult> Index()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return View(activities);
        }

      

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var officer = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officer, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Success = false,
                    Messages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            try
            {
                await _activityService.CreateActivityAsync(model);
                return Json(new
                {
                    Success = true,
                    Message = "Activity created successfully.",
                    RedirectUrl = Url.Action("Index", "Activity")
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    Success = false,
                    Messages = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Messages = new[] {
                    "An unexpected error occurred.",
                    ex.Message
                }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel([FromBody] int id)
        {
            try
            {
                await _activityService.CancelActivityAsync(id);
                return Json(new { success = true, message = "Activity cancelled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                return NotFound(); // Return 404 if the activity doesn't exist
            }
            var officers = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officers, "Id", "Name", activity.OfficerId);


            return View(activity); // Return the full view with the activity model
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    messages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            try
            {
                await _activityService.UpdateActivityAsync(model.ActivityId, model);
                return Json(new
                {
                    success = true,
                    message = "Activity updated successfully.",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, messages = new[] { $"Activity not found: {ex.Message}" } });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, messages = new[] { $"Invalid operation: {ex.Message}" } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, messages = new[] { $"An error occurred while updating the activity: {ex.Message}" } });
            }
        }


    }
}
