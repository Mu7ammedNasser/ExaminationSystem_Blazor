using BlazorApp8.Application.DTOs;
using BlazorApp8.Application.Interfaces;
using BlazorApp8.Domain.Entities;
using System.Threading;

namespace BlazorApp8.Application.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IRepository<Subject> _subjectRepo;
        private readonly IRepository<Exam> _examRepo;
        private readonly IRepository<Question> _questionRepo;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public InstructorService(
            IRepository<Subject> subjectRepo,
            IRepository<Exam> examRepo,
            IRepository<Question> questionRepo)
        {
            _subjectRepo = subjectRepo;
            _examRepo = examRepo;
            _questionRepo = questionRepo;
        }

        private async Task<T> ExecuteWithLockAsync<T>(Func<Task<T>> action)
        {
            await _semaphore.WaitAsync();
            try { return await action(); }
            finally { _semaphore.Release(); }
        }

        public Task<SubjectDto> CreateSubjectAsync(SubjectDto subjectDto) => ExecuteWithLockAsync(async () =>
        {
            var subject = new Subject
            {
                Name = subjectDto.Name,
                Description = subjectDto.Description
            };

            await _subjectRepo.AddAsync(subject);
            await _subjectRepo.SaveChangesAsync();

            subjectDto.Id = subject.Id;
            return subjectDto;
        });

        public Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync() => ExecuteWithLockAsync(async () =>
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            });
        });

        public Task<ExamDto> CreateExamAsync(ExamDto examDto) => ExecuteWithLockAsync(async () =>
        {
            var exam = new Exam
            {
                Title = examDto.Title,
                SubjectId = examDto.SubjectId,
                DurationMinutes = examDto.DurationMinutes,
                TotalQuestions = examDto.TotalQuestions
            };

            await _examRepo.AddAsync(exam);
            await _examRepo.SaveChangesAsync();

            examDto.Id = exam.Id;
            return examDto;
        });

        public Task<IEnumerable<ExamDto>> GetExamsBySubjectAsync(int subjectId) => ExecuteWithLockAsync(async () =>
        {
            var exams = await _examRepo.FindAsync(e => e.SubjectId == subjectId);
            return exams.Select(e => new ExamDto
            {
                Id = e.Id,
                Title = e.Title,
                SubjectId = e.SubjectId,
                DurationMinutes = e.DurationMinutes,
                TotalQuestions = e.TotalQuestions
            });
        });

        public Task<IEnumerable<ExamDto>> GetAllExamsAsync() => ExecuteWithLockAsync(async () =>
        {
            var exams = await _examRepo.GetAllAsync();
            var subjects = await _subjectRepo.GetAllAsync();

            return exams.Select(e => new ExamDto
            {
                Id = e.Id,
                Title = e.Title,
                SubjectId = e.SubjectId,
                SubjectName = subjects.FirstOrDefault(s => s.Id == e.SubjectId)?.Name ?? "",
                DurationMinutes = e.DurationMinutes,
                TotalQuestions = e.TotalQuestions
            });
        });

        public Task<QuestionDto> AddQuestionAsync(QuestionDto questionDto) => ExecuteWithLockAsync(async () =>
        {
            var question = new Question
            {
                ExamId = questionDto.ExamId,
                Text = questionDto.Text,
                OptionA = questionDto.OptionA,
                OptionB = questionDto.OptionB,
                OptionC = questionDto.OptionC,
                OptionD = questionDto.OptionD,
                CorrectAnswer = questionDto.CorrectAnswer
            };

            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveChangesAsync();

            questionDto.Id = question.Id;
            return questionDto;
        });

        public Task<IEnumerable<QuestionDto>> GetQuestionsByExamAsync(int examId) => ExecuteWithLockAsync(async () =>
        {
            var questions = await _questionRepo.FindAsync(q => q.ExamId == examId);
            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                ExamId = q.ExamId,
                Text = q.Text,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer
            });
        });
    }
}
