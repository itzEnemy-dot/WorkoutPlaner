using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Plan
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Ziel { get; set; }

        public bool IsActive { get; set; }




        // Für User brauchen wir 2 Attribute: UserId und UserName, damit wir die Pläne einem User zuordnen können
        public required string UserId { get; set; } //required, damit der Plan immer einem User zugeordnet ist



        [ValidateNever] // Damit wird verhindert, dass der User bei der Validierung des Plans überprüft wird, da wir den User nicht direkt über das Formular eingeben, sondern über die Anmeldung bekommen
        public IdentityUser User { get; set; } // Navigation Property, damit wir den User direkt über den Plan erreichen können


        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>(); //ICollection, damit wir mehrere Übungen in einem Plan haben können

    }
}
