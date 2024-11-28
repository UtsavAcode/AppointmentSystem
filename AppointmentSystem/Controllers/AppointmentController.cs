using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Implementation;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private IAppointmentService _service;
        private IOfficerService _officerService;
        private IVisitorService _visitorService;
        private readonly ILogger<AppointmentController> _logger;
        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService service, IOfficerService officerService, IVisitorService visitorService)
        {
            _service = service;
            _officerService = officerService;
            _visitorService = visitorService;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string officerName, DateTime? date, TimeSpan? startTime, TimeSpan? endTime)
        {

            var appointments = await _service.GetAllAsync();


            if (!string.IsNullOrEmpty(officerName))
            {
                appointments = appointments.Where(a => a.OfficerName.Contains(officerName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (date.HasValue)
            {
                appointments = appointments.Where(a => a.Date.Date == date.Value.Date).ToList();
            }

            if (startTime.HasValue)
            {
                appointments = appointments.Where(a => a.StartTime >= startTime.Value).ToList();
            }

            if (endTime.HasValue)
            {
                appointments = appointments.Where(a => a.EndTime <= endTime.Value).ToList();
            }

            appointments = appointments
                .OrderBy(a => a.Status == AppointmentStatus.Cancelled)
                .ThenBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToList();


            ViewData["OfficerName"] = officerName;
            ViewData["Date"] = date;
            ViewData["StartTime"] = startTime;
            ViewData["EndTime"] = endTime;


            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var officer = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officer, "Id", "Name");
            var visitor = await _visitorService.GetActiveVisitorsAsync();
            ViewBag.Visitors = new SelectList(visitor, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewmodel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var officers = await _officerService.GetActiveOfficersAsync();
                ViewBag.Officers = new SelectList(officers, "Id", "Name");
                var visitors = await _visitorService.GetActiveVisitorsAsync();
                ViewBag.Visitors = new SelectList(visitors, "Id", "Name");
                return View(model);
            }

            try
            {
                await _service.CreateAsync(model);
                TempData["SuccessMessage"] = "Appointment created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the full exception details
                _logger.LogError(ex, "Error creating appointment");

                // Add the specific error message to ModelState
                ModelState.AddModelError(string.Empty, ex.Message);

                // Optionally, add inner exception details if available
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }

                // Repopulate dropdowns
                var officers = await _officerService.GetActiveOfficersAsync();
                ViewBag.Officers = new SelectList(officers, "Id", "Name");

                var visitors = await _visitorService.GetActiveVisitorsAsync();
                ViewBag.Visitors = new SelectList(visitors, "Id", "Name");

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _service.GetAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }


            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return RedirectToAction("Index");
            }


            await _service.CancelAsync(id);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _service.GetAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }


            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "Appointment is cancelled and cannot be edited.";
                return RedirectToAction("Index");
            }


            var model = new EditAppointment
            {
                Id = appointment.Id,
                OfficerId = appointment.OfficerId,
                VisitorId = appointment.VisitorId,
                Name = appointment.Name,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                LastUpdatedOn = appointment.LastUpdatedOn
            };

            var officers = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officers, "Id", "Name");

            var visitors = await _visitorService.GetActiveVisitorsAsync();
            ViewBag.Visitors = new SelectList(visitors, "Id", "Name");


            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> Edit(EditAppointment model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }

            try
            {

                await _service.UpdateAsync(model);
                return Json(new
                {
                    success = true,
                    message = "Appointment updated successfully.",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (InvalidOperationException ex)
            {

                return Json(new
                {
                    success = false,
                    errors = new List<string> { ex.Message }
                });
            }
            catch (KeyNotFoundException ex)
            {

                return Json(new
                {
                    success = false,
                    errors = new List<string> { ex.Message }
                });
            }
            catch (DbUpdateException dbEx)
            {

                var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "A database error occurred.", innerExceptionMessage }
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = false,
                    errors = new List<string> { "An unexpected error occurred.", ex.Message }
                });
            }
        }


        [HttpGet("api/Appointment/GetVisitorAppointments/{visitorId}")]
        public async Task<IActionResult> GetVisitorAppointments(int visitorId)
        {
            try
            {
                var appointments = await _service.GetAppointmentsByVisitorIdAsync(visitorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching visitor {visitorId} appointments");
                return StatusCode(500, "An error occurred while fetching appointments.");
            }
        }

        [HttpGet("api/Appointment/GetOfficerAppointments/{officerId}")]
        public async Task<IActionResult> GetOfficerAppointments(int officerId)
        {
            try
            {

                var appointments = await _service.GetAppointmentsByOfficerIdAsync(officerId);


                return Ok(appointments);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error fetching officer {officerId} appointments");


                return StatusCode(500, "An error occurred while fetching appointments.");
            }


        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusViewModel model)
        {
            try
            {
                await _service.ToggleStatusAsync(model.Id);
                return Json(new { success = true, message = "Status toggled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public class ToggleStatusViewModel
        {
            public int Id { get; set; }
        }



    }
}
