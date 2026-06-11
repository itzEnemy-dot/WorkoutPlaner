
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

[Authorize] // Damit können nur angemeldete User auf die Pläne zugreifen, da die Pläne einem User zugeordnet sind
public class PlanController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PlanController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;

    }

    // GET: PLANS
    public async Task<IActionResult> Index()    
    {
        var userId = _userManager.GetUserId(User);
        return View(await _context.Plans.Where(p => p.UserId == userId).ToListAsync());
    }

    // GET: PLANS/Details/5
    public async Task<IActionResult> Details(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        if (id == null)
        {
            return NotFound();
        }

        var plan = await _context.Plans
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    // GET: PLANS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PLANS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Ziel,IsActive")] Plan plan)
    {
        var userId = _userManager.GetUserId(User);
        plan.Id = Guid.NewGuid();
        plan.UserId = userId;
        ModelState.Remove("UserId"); // UserId wird vom Server gesetzt, nicht vom Formular

        if (ModelState.IsValid)
        {
            _context.Add(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    // GET: PLANS/Edit/5
    public async Task<IActionResult> Edit(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        if (id == null)
        {
            return NotFound();
        }

        var plan = await _context.Plans.FindAsync(id);
        if (plan == null || plan.UserId != userId) //eingefügt
        {
            return NotFound();
        }
        return View(plan);
    }

    // POST: PLANS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? id, [Bind("Id,Name,Ziel,IsActive,UserId")] Plan plan)
    {
        var userId = _userManager.GetUserId(User);
        if (id != plan.Id || plan.UserId != userId) //angepasst, damit nur der User, dem der Plan gehört, den Plan bearbeiten kann
        {
            return NotFound();
        }

        //        if (ModelState.IsValid)
        
        try
        {
            _context.Update(plan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlanExists(plan.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
        
        //        return View(plan);
    }

    // GET: PLANS/Delete/5
    public async Task<IActionResult> Delete(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User); //eingefügt
        if (id == null)
        {
            return NotFound();
        }

        var plan = await _context.Plans
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId); //angepasst, damit nur der User, dem der Plan gehört, den Plan löschen kann

        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    // POST: PLANS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User); //eingefügt

        var plan = await _context.Plans.FindAsync(id);
        if (plan != null && plan.UserId == userId) //angepasst, damit nur der User, dem der Plan gehört, den Plan löschen kann
        {
            _context.Plans.Remove(plan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PlanExists(System.Guid? id)
    {
        return _context.Plans.Any(e => e.Id == id);
    }
}
