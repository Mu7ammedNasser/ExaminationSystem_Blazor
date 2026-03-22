using BlazorApp8.Application.DTOs;

namespace BlazorApp8.Application.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<SubjectDto>> GetSubjectsAsync();
        Task<IEnumerable<ExamDto>> GetExamsForSubjectAsync(int subjectId);
        Task<ExamDto?> GetExamByIdAsync(int examId);
        
        Task<ExamAttemptDto> StartExamAsync(string studentId, int examId);
        Task<IEnumerable<QuestionDto>> GetQuestionsForAttemptAsync(int attemptId);
        
        Task SaveAnswerAsync(int attemptId, int questionId, string selectedAnswer, bool isFlagged);
        Task<ExamAttemptDto> SubmitExamAsync(int attemptId);
        
        Task<ExamAttemptDto?> GetAttemptResultAsync(int attemptId);
        Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsAsync(string studentId);
    }
}
