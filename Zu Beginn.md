Zu Beginn
Visual Studios
ASP.Net Core Web-App MVC
Authentication Type -> Individual Accounts / Einzelne Konten
Erstellen

Nach dem Erstellen
Ausführen / Debuggen
Registrieren -> dann Apply Migration und dann die Seite refreshen

Danach fangen wir an mit die Models
Auf den Ordner Models rechte Maustaste > Add > Class > Erstellen
[ValidateNever] wichtig

Wenn DbSet nicht funktioniert dann schreibe using Microsoft {Projektname}Models;

Add-Migration und dann Update-Database


Controller > Add Controller > with Entity Framework


 [Authorize] // Damit können nur angemeldete User auf die Pläne zugreifen, da die Pläne einem User zugeordnet sind

 das ist ebenfalls wichtig zum eintragen

    private readonly UserManager<IdentityUser> _userManager;

    public PlanController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;

        das muss noch eingefügt werden damit wir den User zuordnen können





                if (plan.UserId != userId)
        {
            return Unauthorized();
        }

        das dann bei der 2. Create Methode damit nur der User seine Pläne bearbeiten kann

        und die if Abfrage mit valid löschen

ViewBag.Plans = new SelectList(plans, "Id", "Name"); eintragen

        