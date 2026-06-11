# WorkoutTracker – Komplette Schritt-für-Schritt Anleitung


---

## Was baut man hier?

Eine Web-App mit der man:
- **Trainingspläne** anlegen kann (nur für den eingeloggten User sichtbar)
- **Übungen** einem Plan zuordnen kann
- **Trainingseinträge** (Datum + Gewicht) zu einer Übung speichern kann

---

## SCHRITT 1 – Projekt in Visual Studio erstellen

1. Visual Studio 2022 öffnen
2. Klick auf **"Neues Projekt erstellen"**
3. Suchfeld oben: `ASP.NET` eintippen
4. Wähle: **"ASP.NET Core-Web-App (Model-View-Controller)"** — muss C# sein
5. Klick **"Weiter"**
6. Projektname: `WorkoutTracker`
7. Speicherort wählen (z.B. Desktop)
8. Klick **"Weiter"**
9. Einstellungen auf der nächsten Seite:
   - Framework: **.NET 10.0** (neueste Version nehmen)
   - Authentifizierungstyp: **"Einzelne Konten"** ← sehr wichtig, sonst kein Login-System!
   - Rest bleibt so
10. Klick **"Erstellen"**

Visual Studio erstellt jetzt automatisch das Grundgerüst der App.

---

## SCHRITT 2 – App zum ersten Mal starten und Datenbank initialisieren

1. Drücke **F5** (oder grüner Play-Button oben)
2. Browser öffnet sich automatisch
3. Klick oben rechts auf **"Register"**
4. E-Mail + Passwort eingeben → auf "Register" klicken
5. Es erscheint ein gelber Balken: **"Apply Migration"** → draufklicken
6. Seite lädt neu → jetzt ist die Datenbank fertig
7. Du kannst dich einloggen

> **Was passiert hier?**  
> ASP.NET erstellt beim ersten Start keine Datenbank automatisch.  
> "Apply Migration" erstellt die Datenbanktabellen für das Login-System.  
> Das macht man nur einmal.

---

## SCHRITT 3 – Die drei Models erstellen

Models beschreiben, **was** in der Datenbank gespeichert wird.  
Jedes Model wird später eine Tabelle in der Datenbank.

Wir brauchen:
- `Plan` – ein Trainingsplan (gehört einem User)
- `Exercise` – eine Übung (gehört einem Plan)
- `LogEntry` – ein Trainingseintrag (gehört einer Übung)

### 3.1 – Model „Plan" erstellen

1. Im **Solution Explorer** (rechts): Rechtsklick auf Ordner **"Models"**
2. **"Hinzufügen"** → **"Klasse"**
3. Name: `Plan.cs` → **"Hinzufügen"**
4. Kompletten Inhalt ersetzen mit:

```csharp
// Diese using-Zeilen werden oben gebraucht
using Microsoft.AspNetCore.Identity;           // für IdentityUser (der eingeloggte User)
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // für [ValidateNever]
using System.ComponentModel.DataAnnotations;   // für [Required], [MaxLength]

namespace WorkoutTracker.Models
{
    public class Plan
    {
        // Eindeutige ID für jeden Plan (Guid = langer zufälliger Schlüssel)
        public Guid Id { get; set; }

        // Name des Plans, z.B. "Push Day" — muss ausgefüllt sein, max 50 Zeichen
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Ziel des Plans, z.B. "Muskelaufbau" — muss ausgefüllt sein, max 50 Zeichen
        [Required]
        [MaxLength(50)]
        public string Ziel { get; set; }

        // Gibt an ob dieser Plan gerade aktiv ist (true/false)
        // Nur bei aktiven Plänen kann man Trainingseinträge erstellen
        public bool IsActive { get; set; }

        // ID des Users, dem dieser Plan gehört
        // "required" = muss immer gesetzt sein, darf nie leer sein
        public required string UserId { get; set; }

        // Navigation Property: der User-Datensatz selbst
        // [ValidateNever] = ASP.NET soll dieses Feld nicht vom Formular erwarten
        // (wird automatisch über UserId gefunden, nicht manuell eingegeben)
        [ValidateNever]
        public IdentityUser User { get; set; }

        // Liste aller Übungen die zu diesem Plan gehören
        // Wird automatisch befüllt wenn man .Include(p => p.Exercises) benutzt
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
```

### 3.2 – Model „Exercise" erstellen

1. Rechtsklick auf **"Models"** → **"Hinzufügen"** → **"Klasse"**
2. Name: `Exercise.cs` → **"Hinzufügen"**
3. Inhalt ersetzen:

```csharp
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Exercise
    {
        // Eindeutige ID für jede Übung
        public Guid Id { get; set; }

        // Name der Übung, z.B. "Bankdrücken"
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Welche Muskelgruppe, z.B. "Brust"
        [Required]
        [MaxLength(50)]
        public string MuscleGroup { get; set; }

        // Anzahl der Sätze, z.B. 4
        [Required]
        public int Sets { get; set; }

        // Anzahl der Wiederholungen pro Satz, z.B. 8
        [Required]
        public int Reps { get; set; }

        // Foreign Key: welchem Plan gehört diese Übung?
        // (speichert die Id des Plans)
        public Guid PlanId { get; set; }

        // Navigation Property: der Plan-Datensatz selbst
        // [ValidateNever] = wird nicht vom Formular erwartet, kommt aus der DB
        [ValidateNever]
        public Plan Plan { get; set; }

        // Liste aller Trainingseinträge für diese Übung
        public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
    }
}
```

### 3.3 – Model „LogEntry" erstellen

1. Rechtsklick auf **"Models"** → **"Hinzufügen"** → **"Klasse"**
2. Name: `LogEntry.cs` → **"Hinzufügen"**
3. Inhalt ersetzen:

```csharp
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class LogEntry
    {
        // Eindeutige ID für jeden Eintrag
        public Guid Id { get; set; }

        // Datum des Trainings, z.B. 11.06.2026
        [Required]
        public DateTime Date { get; set; }

        // Verwendetes Gewicht in kg, z.B. 80.5
        [Required]
        public decimal Weight { get; set; }

        // Foreign Key: zu welcher Übung gehört dieser Eintrag?
        public Guid ExerciseId { get; set; }

        // Navigation Property: die Übung selbst
        // [ValidateNever] = kommt aus der DB, nicht vom Formular
        [ValidateNever]
        public Exercise Exercise { get; set; }
    }
}
```

---

## SCHRITT 4 – DbContext aktualisieren

Der `ApplicationDbContext` ist die **Verbindung zwischen App und Datenbank**.  
Wir müssen ihm sagen, welche Tabellen es geben soll.

1. Ordner **"Data"** öffnen → Datei `ApplicationDbContext.cs` öffnen
2. Die drei `DbSet`-Zeilen hinzufügen:

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models; // ohne diese Zeile findet er Plan, Exercise, LogEntry nicht

namespace WorkoutTracker.Data
{
    // IdentityDbContext enthält bereits die User-Tabellen vom Login-System
    // Wir erweitern ihn um unsere eigenen Tabellen
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // DbSet = eine Tabelle in der Datenbank
        // "Plans" wird der Name der Tabelle in der DB
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
    }
}
```

---

## SCHRITT 5 – Migration ausführen (Tabellen erstellen)

Jetzt sagen wir der Datenbank, dass neue Tabellen erstellt werden sollen.

1. In Visual Studio oben: **"Extras"** → **"NuGet-Paket-Manager"** → **"Paket-Manager-Konsole"**
2. Unten öffnet sich die Konsole. Folgende Befehle nacheinander eingeben:

```
Add-Migration ersterStart
```
→ Enter drücken, warten bis fertig

```
Update-Database
```
→ Enter drücken, warten bis "Done" erscheint

> **Was passiert hier?**  
> `Add-Migration` erstellt eine Datei im Ordner "Migrations" — sie beschreibt welche Tabellen neu gebaut werden sollen.  
> `Update-Database` führt das dann wirklich in der Datenbank aus.  
> Wenn du später neue Felder zu einem Model hinzufügst, musst du das nochmal machen.

---

## SCHRITT 6 – Controller per Scaffolding erstellen

Controller verbinden Models und Views. Visual Studio kann uns einen Basis-Controller automatisch generieren — das nennt sich "Scaffolding".

### 6.1 – PlanController

1. Rechtsklick auf Ordner **"Controllers"**
2. **"Hinzufügen"** → **"Neues Gerüstelement"**
3. Wähle: **"MVC-Controller mit Ansichten unter Verwendung von Entity Framework"**
4. Klick **"Hinzufügen"**
5. Im Dialog:
   - Modellklasse: `Plan (WorkoutTracker.Models)`
   - Datenkontextklasse: `ApplicationDbContext (WorkoutTracker.Data)`
   - Controllername: `PlanController`
6. Klick **"Hinzufügen"**

Visual Studio erstellt jetzt `PlanController.cs` + alle Views automatisch.

### 6.2 – ExerciseController

Gleich wie oben, aber Modellklasse: `Exercise (WorkoutTracker.Models)`

### 6.3 – LogEntryController

Gleich wie oben, aber Modellklasse: `LogEntry (WorkoutTracker.Models)`

---

## SCHRITT 7 – Controller komplett ersetzen

Das Scaffolding erstellt einen Basis-Controller, aber ohne Sicherheit:  
Jeder User könnte die Daten aller anderen User sehen und bearbeiten.  
Wir ersetzen alle drei Controller mit der sicheren, fertigen Version.

### 7.1 – PlanController.cs komplett ersetzen

Öffne `Controllers/PlanController.cs` und ersetze **alles** damit:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;  // für [Authorize]
using Microsoft.AspNetCore.Identity;       // für UserManager

// [Authorize] = nur eingeloggte User dürfen auf diesen Controller zugreifen
// Nicht eingeloggte User werden automatisch zur Login-Seite weitergeleitet
[Authorize]
public class PlanController : Controller
{
    // _context = Zugriff auf die Datenbank
    private readonly ApplicationDbContext _context;

    // _userManager = gibt uns Infos über den eingeloggten User (z.B. seine ID)
    private readonly UserManager<IdentityUser> _userManager;

    // Konstruktor: ASP.NET gibt uns context und userManager automatisch
    // (das nennt sich "Dependency Injection")
    public PlanController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Plan
    // Zeigt alle Pläne des eingeloggten Users
    public async Task<IActionResult> Index()
    {
        // ID des eingeloggten Users holen
        var userId = _userManager.GetUserId(User);

        // Alle Pläne aus der DB holen, aber NUR die vom aktuellen User
        // .Where(...) = Filter: nur Pläne wo UserId mit dem eingeloggten User übereinstimmt
        // .ToListAsync() = Ergebnis als Liste holen (async = wartet bis DB antwortet)
        return View(await _context.Plans
            .Where(p => p.UserId == userId)
            .ToListAsync());
    }

    // GET: /Plan/Details/5
    // Zeigt die Detailseite eines einzelnen Plans
    public async Task<IActionResult> Details(Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        // Wenn keine ID mitgegeben wurde → 404-Fehlerseite
        if (id == null) return NotFound();

        // Plan suchen: muss die richtige ID haben UND dem eingeloggten User gehören
        // Wenn ein anderer User die URL manuell eingibt, bekommt er NotFound
        var plan = await _context.Plans
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (plan == null) return NotFound();

        return View(plan);
    }

    // GET: /Plan/Create
    // Zeigt das leere Formular zum Erstellen eines Plans
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Plan/Create
    // Wird aufgerufen wenn der User das Formular abschickt
    [HttpPost]
    [ValidateAntiForgeryToken] // Schutz vor CSRF-Angriffen (böswillige Formulare von anderen Seiten)
    public async Task<IActionResult> Create([Bind("Name,Ziel,IsActive")] Plan plan)
    // [Bind] = nur diese Felder vom Formular annehmen, den Rest ignorieren
    // UserId kommt NICHT vom Formular — wir setzen sie selbst unten
    {
        var userId = _userManager.GetUserId(User);

        // Neue ID generieren (Guid.NewGuid() = zufälliger eindeutiger Schlüssel)
        plan.Id = Guid.NewGuid();

        // UserId immer vom eingeloggten User setzen — NIEMALS vom Formular!
        // Sonst könnte ein User Pläne für andere User erstellen
        plan.UserId = userId;

        // ModelState enthält Validierungsfehler vom Formular-Empfang
        // Da UserId nicht im Formular war, denkt ASP.NET es fehlt → Fehler
        // .Remove("UserId") sagt: "Den Fehler ignorieren, ich hab UserId selbst gesetzt"
        ModelState.Remove("UserId");

        // Prüfen ob alle Pflichtfelder korrekt ausgefüllt sind
        if (ModelState.IsValid)
        {
            _context.Add(plan);                  // Plan zur DB hinzufügen
            await _context.SaveChangesAsync();   // Änderungen speichern
            return RedirectToAction(nameof(Index)); // Zur Liste weiterleiten
        }

        // Wenn Validierung fehlgeschlagen → Formular nochmal zeigen (mit Fehlermeldungen)
        return View(plan);
    }

    // GET: /Plan/Edit/5
    // Zeigt das Formular zum Bearbeiten eines Plans
    public async Task<IActionResult> Edit(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var plan = await _context.Plans.FindAsync(id);

        // Prüfen: Plan muss existieren UND dem eingeloggten User gehören
        if (plan == null || plan.UserId != userId) return NotFound();

        return View(plan);
    }

    // POST: /Plan/Edit/5
    // Speichert die Änderungen an einem Plan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind("Id,Name,Ziel,IsActive,UserId")] Plan plan)
    {
        var userId = _userManager.GetUserId(User);

        // Doppelte Sicherheitsprüfung:
        // 1. URL-ID muss mit Formular-ID übereinstimmen (kein Manipulation)
        // 2. Plan muss dem eingeloggten User gehören
        if (id != plan.Id || plan.UserId != userId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(plan);               // Plan in der DB aktualisieren
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            // DbUpdateConcurrencyException = zwei User haben gleichzeitig bearbeitet
            {
                if (!PlanExists(plan.Id)) return NotFound();
                else throw; // anderer Fehler → weiterwerfen
            }
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    // GET: /Plan/Delete/5
    // Zeigt die Bestätigungsseite zum Löschen
    public async Task<IActionResult> Delete(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        // Plan suchen — muss dem User gehören
        var plan = await _context.Plans
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (plan == null) return NotFound();

        return View(plan);
    }

    // POST: /Plan/Delete/5
    // Löscht den Plan wirklich aus der Datenbank
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        var plan = await _context.Plans.FindAsync(id);

        // Nur löschen wenn Plan existiert UND dem User gehört
        if (plan != null && plan.UserId == userId)
        {
            _context.Plans.Remove(plan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Hilfsmethode: prüft ob ein Plan mit dieser ID in der DB existiert
    private bool PlanExists(Guid? id)
    {
        return _context.Plans.Any(e => e.Id == id);
    }
}
```

### 7.2 – ExerciseController.cs komplett ersetzen

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering; // für SelectList (Dropdown)

[Authorize]
public class ExerciseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ExerciseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Exercise
    // Zeigt alle Übungen des eingeloggten Users
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        return View(await _context.Exercises
            .Include(e => e.Plan)       // Plan-Daten mitladen (für Planname in der Liste)
            .Where(e => e.Plan.UserId == userId) // nur Übungen von Plänen des Users
            .ToListAsync());
    }

    // GET: /Exercise/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(e => e.Id == id && e.Plan.UserId == userId);

        if (exercise == null) return NotFound();

        return View(exercise);
    }

    // GET: /Exercise/Create
    // Zeigt das Formular — mit Dropdown der eigenen Pläne
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);

        // Alle Pläne des Users für das Dropdown holen
        var plans = await _context.Plans
            .Where(p => p.UserId == userId)
            .ToListAsync();

        // SelectList = Dropdown-Liste für die View
        // "Id" = welcher Wert wird gespeichert, "Name" = was der User sieht
        ViewBag.Plans = new SelectList(plans, "Id", "Name");

        return View();
    }

    // POST: /Exercise/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,MuscleGroup,Sets,Reps,PlanId")] Exercise exercise)
    {
        var userId = _userManager.GetUserId(User);

        // Sicherheitscheck: gehört der gewählte Plan wirklich dem eingeloggten User?
        // Ein User könnte im Browser die PlanId manipulieren
        var ownsplan = await _context.Plans
            .AnyAsync(p => p.Id == exercise.PlanId && p.UserId == userId);

        if (!ownsplan) return NotFound();

        if (ModelState.IsValid)
        {
            exercise.Id = Guid.NewGuid();
            _context.Add(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Bei Fehler: Dropdown neu befüllen (sonst ist es leer wenn Formular nochmal gezeigt wird)
        var plans = await _context.Plans.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.Plans = new SelectList(plans, "Id", "Name", exercise.PlanId);
        return View(exercise);
    }

    // GET: /Exercise/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        // Übung laden — muss über den Plan dem User gehören
        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(e => e.Id == id && e.Plan.UserId == userId);

        if (exercise == null) return NotFound();

        var plans = await _context.Plans.Where(p => p.UserId == userId).ToListAsync();
        // Dritter Parameter "exercise.PlanId" = aktuell ausgewählter Wert im Dropdown
        ViewBag.Plans = new SelectList(plans, "Id", "Name", exercise.PlanId);

        return View(exercise);
    }

    // POST: /Exercise/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind("Id,Name,MuscleGroup,Sets,Reps,PlanId")] Exercise exercise)
    {
        var userId = _userManager.GetUserId(User);
        if (id != exercise.Id) return NotFound();

        // Sicherheitscheck: auch beim Bearbeiten prüfen ob der neue Plan dem User gehört
        var ownsplan = await _context.Plans
            .AnyAsync(p => p.Id == exercise.PlanId && p.UserId == userId);

        if (!ownsplan) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(exercise);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseExists(exercise.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        var plans = await _context.Plans.Where(p => p.UserId == userId).ToListAsync();
        ViewBag.Plans = new SelectList(plans, "Id", "Name", exercise.PlanId);
        return View(exercise);
    }

    // GET: /Exercise/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(m => m.Id == id && m.Plan.UserId == userId);

        if (exercise == null) return NotFound();

        return View(exercise);
    }

    // POST: /Exercise/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid? id)
    {
        var userId = _userManager.GetUserId(User);

        var exercise = await _context.Exercises
            .Include(e => e.Plan)
            .FirstOrDefaultAsync(m => m.Id == id && m.Plan.UserId == userId);

        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
        }
        else
        {
            return NotFound();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExerciseExists(Guid? id)
    {
        return _context.Exercises.Any(e => e.Id == id);
    }
}
```

### 7.3 – LogEntryController.cs komplett ersetzen

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using WorkoutTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class LogEntryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public LogEntryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /LogEntry
    // Zeigt alle Trainingseinträge des eingeloggten Users
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        // await nicht vergessen! Ohne await bekommt die View ein Task-Objekt statt einer Liste
        var logEntries = await _context.LogEntries
            .Include(le => le.Exercise)         // Übung mitladen (für Übungsname in der Liste)
            .ThenInclude(e => e.Plan)           // Plan der Übung mitladen (für UserId-Prüfung)
            .Where(le => le.Exercise.Plan.UserId == userId) // nur Einträge des Users
            .ToListAsync();

        return View(logEntries);
    }

    // GET: /LogEntry/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var logentry = await _context.LogEntries
            .Include(e => e.Exercise)
            .ThenInclude(p => p.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

        if (logentry == null) return NotFound();

        return View(logentry);
    }

    // GET: /LogEntry/Create
    // Zeigt das Formular — nur Übungen von aktiven Plänen im Dropdown
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);

        // Nur Übungen von Plänen holen, die:
        // 1. dem eingeloggten User gehören
        // 2. IsActive = true haben (nur bei aktiven Plänen kann man loggen)
        var exercises = await _context.Exercises
            .Include(e => e.Plan)
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive)
            .ToListAsync();

        ViewBag.Exercises = new SelectList(exercises, "Id", "Name");

        // WICHTIG: return View() ohne Parameter — keine Liste übergeben!
        // Die View erwartet ein LogEntry-Objekt, nicht die exercises-Liste
        return View();
    }

    // POST: /LogEntry/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Date,Weight,ExerciseId")] LogEntry logentry)
    {
        var userId = _userManager.GetUserId(User);

        // Sicherheitscheck: gehört die gewählte Übung dem User und ist der Plan aktiv?
        var allowed = await _context.Exercises
            .AnyAsync(e => e.Id == logentry.ExerciseId
                        && e.Plan.UserId == userId
                        && e.Plan.IsActive);

        if (!allowed) return NotFound();

        if (ModelState.IsValid)
        {
            logentry.Id = Guid.NewGuid();
            _context.Add(logentry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Bei Fehler: Dropdown neu befüllen
        var exercises = await _context.Exercises
            .Include(e => e.Plan)
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive)
            .ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, "Id", "Name", logentry.ExerciseId);
        return View(logentry);
    }

    // GET: /LogEntry/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var logentry = await _context.LogEntries
            .Include(l => l.Exercise)
            .ThenInclude(e => e.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

        if (logentry == null) return NotFound();

        var exercises = await _context.Exercises
            .Include(e => e.Plan)
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive)
            .ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, "Id", "Name", logentry.ExerciseId);

        return View(logentry);
    }

    // POST: /LogEntry/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind("Id,Date,Weight,ExerciseId")] LogEntry logentry)
    {
        var userId = _userManager.GetUserId(User);
        if (id != logentry.Id) return NotFound();

        var allowed = await _context.Exercises
            .AnyAsync(e => e.Id == logentry.ExerciseId
                        && e.Plan.UserId == userId
                        && e.Plan.IsActive);

        if (!allowed) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(logentry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogEntryExists(logentry.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        var exercises = await _context.Exercises
            .Include(e => e.Plan)
            .Where(e => e.Plan.UserId == userId && e.Plan.IsActive)
            .ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, "Id", "Name", logentry.ExerciseId);
        return View(logentry);
    }

    // GET: /LogEntry/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        var userId = _userManager.GetUserId(User);
        if (id == null) return NotFound();

        var logentry = await _context.LogEntries
            .Include(l => l.Exercise)
            .ThenInclude(e => e.Plan)
            .FirstOrDefaultAsync(l => l.Id == id && l.Exercise.Plan.UserId == userId);

        if (logentry == null) return NotFound();

        return View(logentry);
    }

    // POST: /LogEntry/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid? id)
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

    private bool LogEntryExists(Guid? id)
    {
        return _context.LogEntries.Any(e => e.Id == id);
    }
}
```

---

## SCHRITT 8 – Views anpassen

Das Scaffolding erstellt Views die rohe GUIDs anzeigen und interne Felder wie UserId im Formular zeigen. Wir ersetzen die wichtigsten Views.

### 8.1 – Views/Plan/Create.cshtml

```cshtml
@model WorkoutTracker.Models.Plan

@{
    ViewData["Title"] = "Neuer Plan";
}

<h1>Neuen Trainingsplan erstellen</h1>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>

            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="Ziel" class="control-label"></label>
                <input asp-for="Ziel" class="form-control" />
                <span asp-validation-for="Ziel" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="IsActive" class="control-label">Aktiver Plan?</label>
                <input asp-for="IsActive" class="form-check-input" />
            </div>

            {{!-- UserId und User sind hier NICHT drin --}}
            {{!-- Der Controller setzt UserId selbst vom eingeloggten User --}}

            <div class="form-group mt-3">
                <input type="submit" value="Erstellen" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>
<div>
    <a asp-action="Index">Zurück zur Liste</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### 8.2 – Views/Plan/Index.cshtml

```cshtml
@model IEnumerable<WorkoutTracker.Models.Plan>

@{
    ViewData["Title"] = "Meine Trainingspläne";
}

<h1>Meine Trainingspläne</h1>

<p>
    <a asp-action="Create" class="btn btn-success">+ Neuer Plan</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Ziel</th>
            <th>Aktiv</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@Html.DisplayFor(model => item.Name)</td>
                <td>@Html.DisplayFor(model => item.Ziel)</td>
                <td>@Html.DisplayFor(model => item.IsActive)</td>
                <td>
                    <a asp-action="Edit" asp-route-id="@item.Id">Bearbeiten</a> |
                    <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                    <a asp-action="Delete" asp-route-id="@item.Id">Löschen</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 8.3 – Views/Exercise/Index.cshtml

```cshtml
@model IEnumerable<WorkoutTracker.Models.Exercise>

@{
    ViewData["Title"] = "Meine Übungen";
}

<h1>Meine Übungen</h1>

<p>
    <a asp-action="Create" class="btn btn-success">+ Neue Übung</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Muskelgruppe</th>
            <th>Sätze</th>
            <th>Wiederholungen</th>
            <th>Plan</th>  @* Plan-Name statt roher GUID anzeigen *@
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@Html.DisplayFor(model => item.Name)</td>
                <td>@Html.DisplayFor(model => item.MuscleGroup)</td>
                <td>@Html.DisplayFor(model => item.Sets)</td>
                <td>@Html.DisplayFor(model => item.Reps)</td>
                <td>@item.Plan?.Name</td>  @* item.Plan.Name statt item.PlanId *@
                <td>
                    <a asp-action="Edit" asp-route-id="@item.Id">Bearbeiten</a> |
                    <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                    <a asp-action="Delete" asp-route-id="@item.Id">Löschen</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 8.4 – Views/LogEntry/Index.cshtml

```cshtml
@model IEnumerable<WorkoutTracker.Models.LogEntry>

@{
    ViewData["Title"] = "Trainingsprotokoll";
}

<h1>Trainingsprotokoll</h1>

<p>
    <a asp-action="Create" class="btn btn-success">+ Eintrag hinzufügen</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>Datum</th>
            <th>Gewicht (kg)</th>
            <th>Übung</th>  @* Übungsname statt roher GUID *@
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@item.Date.ToString("dd.MM.yyyy")</td>  @* Datum formatieren: 11.06.2026 *@
                <td>@Html.DisplayFor(model => item.Weight)</td>
                <td>@item.Exercise?.Name</td>  @* item.Exercise.Name statt item.ExerciseId *@
                <td>
                    <a asp-action="Edit" asp-route-id="@item.Id">Bearbeiten</a> |
                    <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                    <a asp-action="Delete" asp-route-id="@item.Id">Löschen</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 8.5 – Views/LogEntry/Create.cshtml

```cshtml
@model WorkoutTracker.Models.LogEntry

@{
    ViewData["Title"] = "Trainingseintrag";
}

<h1>Neuen Trainingseintrag erstellen</h1>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>

            <div class="form-group">
                <label asp-for="Date" class="control-label">Datum</label>
                @* type="date" = Datepicker im Browser *@
                <input asp-for="Date" class="form-control" type="date" />
                <span asp-validation-for="Date" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="Weight" class="control-label">Gewicht (kg)</label>
                <input asp-for="Weight" class="form-control" />
                <span asp-validation-for="Weight" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="ExerciseId" class="control-label">Übung</label>
                @* asp-items="ViewBag.Exercises" = Dropdown-Optionen kommen aus dem Controller *@
                <select asp-for="ExerciseId" class="form-control" asp-items="ViewBag.Exercises">
                    <option value="">-- Übung auswählen --</option>
                </select>
                <span asp-validation-for="ExerciseId" class="text-danger"></span>
            </div>

            <div class="form-group mt-3">
                <input type="submit" value="Speichern" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>
<div>
    <a asp-action="Index">Zurück zur Liste</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

## SCHRITT 9 – Navigation anpassen

Öffne `Views/Shared/_Layout.cshtml`.  
Suche den `<ul class="navbar-nav ...">` Block und ersetze den Inhalt:

```cshtml
<ul class="navbar-nav flex-grow-1">
    <li class="nav-item">
        <a class="nav-link text-dark" asp-controller="Home" asp-action="Index">Home</a>
    </li>

    @* Alle App-Links nur für eingeloggte User anzeigen *@
    @if (User.Identities.Any(identity => identity.IsAuthenticated))
    {
        <li class="nav-item">
            <a class="nav-link text-dark" asp-controller="Plan" asp-action="Index">Pläne</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-controller="Exercise" asp-action="Index">Übungen</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-controller="LogEntry" asp-action="Index">Protokoll</a>
        </li>
    }
</ul>
```

---

## SCHRITT 10 – Fertig! So testest du die App

1. **F5** drücken um die App zu starten
2. **Registrieren** (oben rechts "Register")
3. **Plan erstellen**: Pläne → Neuer Plan → Name + Ziel eingeben, "Aktiv" anhaken → Erstellen
4. **Übung erstellen**: Übungen → Neue Übung → Felder ausfüllen, Plan aus Dropdown wählen → Erstellen
5. **Trainingseintrag**: Protokoll → Eintrag hinzufügen → Datum + Gewicht + Übung wählen → Speichern

> **Wichtig:** Im Dropdown bei "Eintrag hinzufügen" erscheinen nur Übungen von **aktiven** Plänen.  
> Wenn das Dropdown leer ist → Plan bearbeiten und "Aktiv" anhaken.

---

## Häufige Fehler & Lösungen

| Fehler | Ursache | Lösung |
|--------|---------|--------|
| "Apply Migration" erscheint beim ersten Start | Datenbank noch nicht erstellt | Draufklicken, Seite lädt neu |
| Plan lässt sich nicht speichern | `ModelState.Remove("UserId")` fehlt im Controller | Controller-Code aus Schritt 7.1 nochmal prüfen |
| `InvalidOperationException: model item of type List<Exercise>` | `return View(exercises)` statt `return View()` | In LogEntryController.Create GET: `return View()` ohne Parameter |
| Dropdown bei LogEntry/Create ist leer | Plan ist nicht auf aktiv gesetzt | Plan bearbeiten → "Aktiver Plan?" Haken setzen |
| `DbSet` wird nicht erkannt | `using WorkoutTracker.Models` fehlt | Oben in `ApplicationDbContext.cs` eintragen |
| Andere User sehen meine Daten | `[Authorize]` oder `UserId`-Filter fehlt | Controller aus Schritt 7 nochmal vergleichen |
