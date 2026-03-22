namespace BlazorApp8.Application.DTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class ExamAttemptDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsSubmitted { get; set; }
        
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }

    public class StudentAnswerDto
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; } = string.Empty;
        public bool IsFlagged { get; set; }
    }
}
