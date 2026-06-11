using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WorkoutTracker.Models
{
    public class LogEntry
    {

        public Guid Id { get; set;}

        public DateTime Date { get; set; }

        public decimal Weight { get; set; }

        public Guid ExerciseId { get; set; } // Foreign Key, damit wir den LogEntry einer Übung zuordnen können

        [ValidateNever]
        public Exercise Exercise { get; set; } // Navigation Property, damit wir die Übung direkt über den LogEntry erreichen können


    }
}
