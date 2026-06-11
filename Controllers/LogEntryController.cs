
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

[Authorize] // Damit können nur angemeldete User auf die Pläne zugreifen, da die Pläne einem User zugeordnet sind

public class LogEntryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public LogEntryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: LOGENTRYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.LogEntries.ToListAsync());
    }

    // GET: LOGENTRYS/Details/5
    public async Task<IActionResult> Details(System.Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var logentry = await _context.LogEntries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (logentry == null)
        {
            return NotFound();
        }

        return View(logentry);
    }

    // GET: LOGENTRYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LOGENTRYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Date,Weight,ExerciseId")] LogEntry logentry)
    {
        if (ModelState.IsValid)
        {
            _context.Add(logentry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(logentry);
    }

    // GET: LOGENTRYS/Edit/5
    public async Task<IActionResult> Edit(System.Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var logentry = await _context.LogEntries.FindAsync(id);
        if (logentry == null)
        {
            return NotFound();
        }
        return View(logentry);
    }

    // POST: LOGENTRYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? id, [Bind("Id,Date,Weight,ExerciseId")] LogEntry logentry)
    {
        if (id != logentry.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(logentry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogEntryExists(logentry.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(logentry);
    }

    // GET: LOGENTRYS/Delete/5
    public async Task<IActionResult> Delete(System.Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var logentry = await _context.LogEntries
            .FirstOrDefaultAsync(m => m.Id == id);
        if (logentry == null)
        {
            return NotFound();
        }

        return View(logentry);
    }

    // POST: LOGENTRYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(System.Guid? id)
    {
        var logentry = await _context.LogEntries.FindAsync(id);
        if (logentry != null)
        {
            _context.LogEntries.Remove(logentry);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LogEntryExists(System.Guid? id)
    {
        return _context.LogEntries.Any(e => e.Id == id);
    }
}
