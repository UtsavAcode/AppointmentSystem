using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

public class VisitorController : Controller
{
    private readonly IVisitorService _visitorService;

    public VisitorController(IVisitorService visitorService)
    {
        _visitorService = visitorService;
    }

    public async Task<IActionResult> Index()
    {
        var visitors = await _visitorService.GetAllVisitorsAsync();
        return View(visitors);
    }

    public IActionResult Create()
    {
        return View(new VisitorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,MobileNumber,EmailAddress")] VisitorViewModel visitorViewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _visitorService.CreateVisitorAsync(visitorViewModel);
                TempData["Success"] = "Visitor created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating visitor: " + ex.Message);
            }
        }
        return View(visitorViewModel);
    }

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
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,MobileNumber,EmailAddress,Status")] VisitorViewModel visitorViewModel)
    {
        if (id != visitorViewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _visitorService.UpdateVisitorAsync(visitorViewModel);
                TempData["Success"] = "Visitor updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await VisitorExists(id))
                {
                    return NotFound();
                }
                ModelState.AddModelError("", "Error updating visitor: " + ex.Message);
            }
        }
        return View(visitorViewModel);
    }

    private async Task<bool> VisitorExists(int id)
    {
        var visitor = await _visitorService.GetVisitorByIdAsync(id);
        return visitor != null;
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id, [FromBody] bool status)
    {
        var visitor = await _visitorService.GetVisitorByIdAsync(id);
        if (visitor == null)
        {
            return NotFound();
        }

        visitor.Status = status;
        await _visitorService.UpdateVisitorAsync(visitor);

        return Ok(new { success = true });
    }


}