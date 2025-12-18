namespace pomodoro_api.Models
{
    public class PomodoroSession
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public TaskItem? Task { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; } = 25;
        public bool Completed { get; set; }
    }
}
