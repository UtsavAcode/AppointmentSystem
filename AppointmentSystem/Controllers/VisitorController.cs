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
    public async Task<IActionResult> Edit(VisitorViewModel model)
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

            // Update visitor properties
            existingVisitor.Name = model.Name;
            existingVisitor.MobileNumber = model.MobileNumber;
            existingVisitor.EmailAddress = model.EmailAddress;
            existingVisitor.Status = model.Status; 

            await _visitorService.UpdateVisitorAsync(existingVisitor);

            TempData["SuccessMessage"] = "Visitor updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error updating visitor: " + ex.Message);
            return View(model);
        }
    }





    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id, bool status)
    {
        try
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound(new { message = "Visitor not found" });
            }

            // Update the status including related appointments
            visitor.Status = status;
            await _visitorService.UpdateVisitorAsync(visitor);


            if (!status)
            {
                await _visitorService.DeactivateFutureAppointmentsAsync(id);  // Call the method to deactivate future appointments
            }
            else
            {
                await _visitorService.ReactivateFutureAppointmentsAsync(id);  // Reactivate future appointments
            }

            return Ok(new
            {
                success = true,
                message = $"Visitor status has been {(status ? "activated" : "deactivated")} successfully",
                status = status
            });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the status.",
                error = ex.Message
            });
        }
    }



}