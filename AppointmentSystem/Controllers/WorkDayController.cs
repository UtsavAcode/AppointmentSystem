using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Implementation;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Controllers
{
    public class WorkDayController : Controller
    {
        private IWorkdaysService _service;
        private IOfficerService _officerService;

        public WorkDayController(IWorkdaysService service, IOfficerService officerService)
        {
            _service = service;
            _officerService = officerService;
        }
        public async Task<IActionResult> Index()
        {
            var days = await _service.GetAllWorkDaysAsync();
            return View(days);
        }

        public async Task<IActionResult> AddWorkDay()
        {
            var officer = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officer, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkDay(WorkDayViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Work model is null");
            }

            await _service.AddWorkDayAsync(model);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int id)
        {
           
            var workDay = await _service.GetWorkByIdAsync(id); 

            if (workDay == null)
            {
                return NotFound();
            }

           
            var model = new WorkDayViewModel
            {
                Id = workDay.Id,
                DayOfWeek = workDay.DayOfWeek,
                OfficerId = workDay.OfficerId
            };

            
            var officers = await _officerService.GetActiveOfficersAsync();
            ViewBag.Officers = new SelectList(officers, "Id", "Name", model.OfficerId); 

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(WorkDayViewModel model)
        {
            if (!ModelState.IsValid)
            {
               
                return View(model);
            }

            try
            {
                await _service.UpdateWorkDayAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
      
        }
    
}
