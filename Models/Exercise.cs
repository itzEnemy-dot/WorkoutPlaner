using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Exercise
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string MuscleGroup { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }

        public Guid PlanId { get; set; } // Foreign Key, damit wir die Übung einem Plan zuordnen können

        public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>(); //ICollection, damit wir mehrere LogEntries in einer Übung haben können

        [ValidateNever]
        public Plan Plan { get; set; } // Navigation Property, damit wir den Plan direkt über die Übung erreichen können



    }
}
