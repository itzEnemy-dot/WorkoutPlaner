
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
        var userId = _userManager.GetUserId(User);
        var logEntries = await _context.LogEntries
            .Include(le => le.Exercise) //joint die LogEntries mit den Übungen, damit wir die Übung direkt über die LogEntry erreichen können
            .ThenInclude(e => e.Plan) //joint die Übungen mit den Plänen, damit wir den Plan direkt über die Übung erreichen können
            .Where(le => le.Exercise.Plan.UserId == userId) //filtert die LogEntries, damit nur die LogEntries angezeigt werden, die einem Plan zugeordnet sind, der dem aktuellen User gehört
            .ToListAsync();

        return View(logEntries);
    }

    // GET: LOGENTRYS/Details/5
    public async Task<IActionResult> Details(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null)
        {
            return NotFound();
        }

        var logentry = await _context.LogEntries
            .Include(e => e.Exercise)
            .ThenInclude(p => p.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

        if (logentry == null)
        {
            return NotFound();
        }

        return View(logentry);
    }

    // GET: LOGENTRYS/Create
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);
        var exercises = await _context.Exercises
            .Include(e => e.Plan) //joint die Übungen mit den Plänen, damit wir den Plan direkt über die Übung erreichen können
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive) //filtert die Übungen, damit nur die Übungen angezeigt werden, die einem Plan zugeordnet sind, der dem aktuellen User gehört
            .ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, "Id", "Name"); // Damit können wir die Übungen in einem Dropdown-Menü anzeigen, damit der User die Übung auswählen kann, der die LogEntry zugeordnet werden soll
        return View();
    }

    // POST: LOGENTRYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Date,Weight,ExerciseId")] LogEntry logentry)
    {
        var userId = _userManager.GetUserId(User);

        
       var allowed = await _context.Exercises.AnyAsync(e => e.Id == logentry.ExerciseId && e.Plan.UserId == userId && e.Plan.IsActive);//prüft, ob die Übung, die der User ausgewählt hat, einem Plan zugeordnet ist, der dem aktuellen User gehört und aktiv ist
       
        if (!allowed)
        {
            return NotFound();
        }
        
        
        logentry.Id = Guid.NewGuid(); // Generiert eine neue Id für die LogEntry, da die Id in der Datenbank als Primary Key definiert ist und automatisch generiert werden muss
        _context.Add(logentry);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));


        }
 
    

    // GET: LOGENTRYS/Edit/5
    public async Task<IActionResult> Edit(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        if (id == null)
        {
            return NotFound();
        }


        var logentry = await _context.LogEntries
            .Include(l => l.Exercise)
            .ThenInclude(e => e.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

        if (logentry == null)
        {
            return NotFound();
        }

        var exercises = await _context.Exercises
            .Include(e => e.Plan)
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive)
            .ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, "Id", "Name");

        return View(logentry);
    }

    // POST: LOGENTRYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? id, [Bind("Id,Date,Weight,ExerciseId")] LogEntry logentry)
    {
        var userId = _userManager.GetUserId(User);
        if (id != logentry.Id)
        {
            return NotFound();
        }

        var allowed = await _context.Exercises.AnyAsync(e => e.Id == logentry.ExerciseId && e.Plan.UserId == userId && e.Plan.IsActive); //prüft, ob die Übung, die der User ausgewählt hat, einem Plan zugeordnet ist, der dem aktuellen User gehört und aktiv ist


        if (!allowed)
        {
            return NotFound();
        }

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

    // GET: LOGENTRYS/Delete/5
    public async Task<IActionResult> Delete(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        if (id == null)
        {
            return NotFound();
        }

        var logentry = await _context.LogEntries
            .Include(l => l.Exercise)
            .ThenInclude(e => e.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

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
        var userId = _userManager.GetUserId(User);
        var logentry = await _context.LogEntries
            .Include(l => l.Exercise)
            .ThenInclude(e => e.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

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
