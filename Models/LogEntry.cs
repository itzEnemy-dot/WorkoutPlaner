namespace WorkoutTracker.Models
{
    public class LogEntry
    {

        public Guid Id { get; set;}

        public DateTime Date { get; set; }

        public decimal Weight { get; set; }

        public Guid ExerciseId { get; set; } // Foreign Key, damit wir den LogEntry einer Übung zuordnen können


    }
}
