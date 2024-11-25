using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private IAppointmentService _service;
        private IOfficerService _officerService;
        private IVisitorService _visitorService;
        private readonly ILogger<AppointmentController> _logger;
        public AppointmentController(ILogger<AppointmentController> logger,IAppointmentService service, IOfficerService officerService, IVisitorService visitorService)
        {
            _service = service;
            _officerService = officerService;
            _visitorService = visitorService;
            _logger = logger; 
        }
        public async Task<IActionResult> Index()
        {
           var appointment =  await _service.GetAllAsync();
            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var officer =await _officerService.GetActiveOfficersAsync();
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


    }
}
