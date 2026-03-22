namespace BlazorApp8.Domain.Entities
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        public int ExamAttemptId { get; set; }
        public ExamAttempt? ExamAttempt { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        public string SelectedAnswer { get; set; } = string.Empty; // "A", "B", "C", "D" or empty
        public bool IsFlagged { get; set; }
    }
}
