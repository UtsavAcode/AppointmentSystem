using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Controllers
{
    public class VisitorController : Controller
    {



        private readonly IVisitorService _visitorService;

        public VisitorController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        // Display a list of all visitors
        public async Task<IActionResult> Index()
        {
            var visitors = await _visitorService.GetAllVisitorsAsync();
            return View(visitors);  // Return the list to the view
        }

        // Display the details of a single visitor
        public async Task<IActionResult> Details(int id)
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            return View(visitor);  // Return the visitor details to the view
        }

        // Display the Create view (empty form)
        public IActionResult Create()
        {
            return View();  // Return the form view for creating a visitor
        }

        // Handle the POST request to create a new visitor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitorViewModel visitorViewModel)
        {
            if (ModelState.IsValid)
            {
                await _visitorService.CreateVisitorAsync(visitorViewModel);
                return RedirectToAction(nameof(Index));  // Redirect to the Index (list) view after successful creation
            }
            return View(visitorViewModel);  // Return the form with validation errors
        }

        // Display the Edit view with an existing visitor's data
        public async Task<IActionResult> Edit(int id)
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            return View(visitor);  // Return the form with the existing visitor data
        }

        // Handle the POST request to update a visitor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitorViewModel visitorViewModel)
        {
            if (id != visitorViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _visitorService.UpdateVisitorAsync(visitorViewModel);
                return RedirectToAction(nameof(Index));  // Redirect to the Index (list) view after successful update
            }
            return View(visitorViewModel);  // Return the form with validation errors
        }

        // Handle the request to delete a visitor


    }

}
