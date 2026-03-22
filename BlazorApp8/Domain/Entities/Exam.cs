namespace BlazorApp8.Domain.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
    }
}
