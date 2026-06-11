
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize] // Damit können nur angemeldete User auf die Pläne zugreifen, da die Pläne einem User zugeordnet sind

public class ExerciseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ExerciseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: EXERCISES
    public async Task<IActionResult> Index()    
    {
        var userId = _userManager.GetUserId(User);
        return View(await _context.Exercises.Include(e => e.Plan).Where(e => e.Plan.UserId  == userId).ToListAsync());
    }

    // GET: EXERCISES/Details/5
    public async Task<IActionResult> Details(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null)
        {
            return NotFound();
        }

        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(e => e.Id == id && e.Plan.UserId == userId);
        if (exercise == null)
        {
            return NotFound();
        }

        return View(exercise);
    }

    // GET: EXERCISES/Create
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);
        var plans = await _context.Plans.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.Plans = new SelectList(plans, "Id", "Name"); // Damit können wir die Pläne in einem Dropdown-Menü anzeigen, damit der User den Plan auswählen kann, dem die Übung zugeordnet werden soll

        return View();
    }
    

    // POST: EXERCISES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,MuscleGroup,Sets,Reps, PlanId")] Exercise exercise) //Das was der User eingeben soll
    {
        var userId = _userManager.GetUserId(User);
        var ownsplan = await _context.Plans.AnyAsync(p => p.Id == exercise.PlanId && p.UserId == userId); // Überprüfen, ob der Plan, dem die Übung zugeordnet wird, dem Benutzer gehört
        
        if (!ownsplan)
        {
            return NotFound("User is not the owner of the selected plan."); // Wenn der Plan nicht dem Benutzer gehört, wird eine NotFound-Antwort zurückgegeben
        }

        exercise.Id = Guid.NewGuid(); // Generiere eine neue Id für die Übung
        //exercise.PlanId = new Guid(); // Weise der Übung eine neue PlanId zu


        _context.Add(exercise);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
        

    }

    // GET: EXERCISES/Edit/5
    public async Task<IActionResult> Edit(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null)
        {
            return NotFound();
        }

        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(e => e.Id == id && e.Plan.UserId == userId);
        if (exercise == null)
        {
            return NotFound();
        }

        var plans = await _context.Plans.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.Plans = new SelectList(plans, "Id", "Name",exercise.PlanId); // Damit können wir die Pläne in einem Dropdown-Menü anzeigen, damit der User den Plan auswählen kann, dem die Übung zugeordnet werden soll

        return View(exercise);
    }

    // POST: EXERCISES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? id, [Bind("Id,Name,MuscleGroup,Sets,Reps,PlanId")] Exercise exercise)
    {
        var userId = _userManager.GetUserId(User);
        if (id != exercise.Id)
        {
            return NotFound();
        }

        var ownsplan = await _context.Plans.AnyAsync(p => p.Id == exercise.PlanId && p.UserId == userId); // Überprüfen, ob der Plan, dem die Übung zugeordnet
        if (!ownsplan) {

            return NotFound("User is not the owner of the selected plan."); // Wenn der Plan nicht dem Benutzer gehört, wird eine NotFound-Antwort zurückgegeben

        try
        {
            _context.Update(exercise);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExerciseExists(exercise.Id))
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
        return View(exercise);
    }

    // GET: EXERCISES/Delete/5
    public async Task<IActionResult> Delete(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null)
        {
            return NotFound();
        }

        var exercise = await _context.Exercises
            .Include(e => e.Plan) // Damit können wir überprüfen, ob die Übung einem Plan zugeordnet ist, der dem Benutzer gehört
            .FirstOrDefaultAsync(m => m.Id == id && m.Plan.UserId == userId);// Überprüfen, ob die Übung, die gelöscht werden soll, einem Plan zugeordnet ist, der dem Benutzer gehört
        if (exercise == null)
        {
            return NotFound();
        }

        return View(exercise);
    }

    // POST: EXERCISES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(System.Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        var exercise = await _context.Exercises
            .Include(e => e.Plan) // 
            .FirstOrDefaultAsync(m => m.Id == id && m.Plan.UserId == userId);

        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
        }else{

            return NotFound();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExerciseExists(System.Guid? id)
    {
        return _context.Exercises.Any(e => e.Id == id);
    }
}
