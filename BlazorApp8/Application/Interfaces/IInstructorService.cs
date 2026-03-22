using BlazorApp8.Application.DTOs;

namespace BlazorApp8.Application.Interfaces
{
    public interface IInstructorService
    {
        Task<SubjectDto> CreateSubjectAsync(SubjectDto subjectDto);
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        
        Task<ExamDto> CreateExamAsync(ExamDto examDto);
        Task<IEnumerable<ExamDto>> GetExamsBySubjectAsync(int subjectId);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();
        
        Task<QuestionDto> AddQuestionAsync(QuestionDto questionDto);
        Task<IEnumerable<QuestionDto>> GetQuestionsByExamAsync(int examId);
    }
}
