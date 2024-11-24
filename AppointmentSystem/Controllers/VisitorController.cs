using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Implementation;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

public class VisitorController : Controller
{
    private readonly IVisitorService _visitorService;

    public VisitorController(IVisitorService visitorService)
    {
        _visitorService = visitorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var visitors = await _visitorService.GetAllVisitorsAsync();
        return View(visitors);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VisitorViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _visitorService.CreateVisitorAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating visitor: " + ex.Message);
            }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var visitor = await _visitorService.GetVisitorByIdAsync(id);
        if (visitor == null)
        {
            return NotFound();
        }
        return View(visitor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit( VisitorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            var existingVisitor = await _visitorService.GetVisitorByIdAsync(model.Id);
            if (existingVisitor == null)
            {
                return NotFound();
            }
            // Update all fields including Status
            existingVisitor.Name = model.Name;
            existingVisitor.MobileNumber = model.MobileNumber;
            existingVisitor.EmailAddress = model.EmailAddress;
          // Preserve the existing status

            await _visitorService.UpdateVisitorAsync(existingVisitor);
        
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error updating visitor: " + ex.Message);
            return View(model);
        }
    }


    private async Task<bool> VisitorExists(int id)
    {
        var visitor = await _visitorService.GetVisitorByIdAsync(id);
        return visitor != null;
    }


    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id, bool status)
    {
        try
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound(); // Post not found, return 404
            }

            visitor.Status = status;
            await _visitorService.UpdateVisitorAsync(visitor); // Ensure this method works properly
            return Ok(); // Status updated successfully
        }
        catch (Exception ex)
        {
            // Log the exception here if needed for debugging
            return StatusCode(500, "An error occurred while updating the status. " + ex.Message);
        }
    }




}