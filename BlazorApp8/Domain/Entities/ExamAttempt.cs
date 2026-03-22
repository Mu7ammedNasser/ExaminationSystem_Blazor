namespace BlazorApp8.Domain.Entities
{
    public class ExamAttempt
    {
        public int Id { get; set; }
        
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Score { get; set; }
        public bool IsSubmitted { get; set; }

        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
