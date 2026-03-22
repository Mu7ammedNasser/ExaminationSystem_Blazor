using BlazorApp8.Application.DTOs;
using BlazorApp8.Application.Interfaces;
using BlazorApp8.Domain.Entities;
using System.Threading;

namespace BlazorApp8.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Subject> _subjectRepo;
        private readonly IRepository<Exam> _examRepo;
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<ExamAttempt> _attemptRepo;
        private readonly IRepository<StudentAnswer> _answerRepo;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public StudentService(
            IRepository<Subject> subjectRepo,
            IRepository<Exam> examRepo,
            IRepository<Question> questionRepo,
            IRepository<ExamAttempt> attemptRepo,
            IRepository<StudentAnswer> answerRepo)
        {
            _subjectRepo = subjectRepo;
            _examRepo = examRepo;
            _questionRepo = questionRepo;
            _attemptRepo = attemptRepo;
            _answerRepo = answerRepo;
        }

        private async Task<T> ExecuteWithLockAsync<T>(Func<Task<T>> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await action();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task ExecuteWithLockAsync(Func<Task> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task<IEnumerable<SubjectDto>> GetSubjectsAsync() => ExecuteWithLockAsync(async () =>
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            });
        });

        public Task<IEnumerable<ExamDto>> GetExamsForSubjectAsync(int subjectId) => ExecuteWithLockAsync(async () =>
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

        public Task<ExamDto?> GetExamByIdAsync(int examId) => ExecuteWithLockAsync(async () =>
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null) return (ExamDto?)null;

            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                SubjectId = exam.SubjectId,
                DurationMinutes = exam.DurationMinutes,
                TotalQuestions = exam.TotalQuestions
            };
        });

        public Task<ExamAttemptDto> StartExamAsync(string studentId, int examId) => ExecuteWithLockAsync(async () =>
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null) throw new Exception("Exam not found");

            var attempts = await _attemptRepo.FindAsync(a => a.StudentId == studentId && a.ExamId == examId);
            var existing = attempts.FirstOrDefault();

            if (existing != null)
            {
                if (existing.IsSubmitted)
                {
                    throw new Exception("You have already completed this exam.");
                }

                return new ExamAttemptDto
                {
                    Id = existing.Id,
                    StudentId = existing.StudentId,
                    ExamId = existing.ExamId,
                    StartTime = existing.StartTime,
                    TotalQuestions = exam.TotalQuestions
                };
            }

            var attempt = new ExamAttempt
            {
                StudentId = studentId,
                ExamId = examId,
                StartTime = DateTime.UtcNow,
                IsSubmitted = false,
                Score = 0
            };

            await _attemptRepo.AddAsync(attempt);
            await _attemptRepo.SaveChangesAsync();

            var allQuestions = (await _questionRepo.FindAsync(q => q.ExamId == examId)).ToList();
            int questionLimit = exam.TotalQuestions > 0 ? exam.TotalQuestions : 10;
            var randomQuestions = allQuestions.OrderBy(q => Guid.NewGuid()).Take(questionLimit).ToList();

            foreach (var q in randomQuestions)
            {
                var answer = new StudentAnswer
                {
                    ExamAttemptId = attempt.Id,
                    QuestionId = q.Id,
                    SelectedAnswer = "",
                    IsFlagged = false
                };
                await _answerRepo.AddAsync(answer);
            }
            await _answerRepo.SaveChangesAsync();

            return new ExamAttemptDto
            {
                Id = attempt.Id,
                StudentId = attempt.StudentId,
                ExamId = attempt.ExamId,
                StartTime = attempt.StartTime,
                TotalQuestions = randomQuestions.Count
            };
        });

        public Task<IEnumerable<QuestionDto>> GetQuestionsForAttemptAsync(int attemptId) => ExecuteWithLockAsync(async () =>
        {
            var answers = await _answerRepo.FindAsync(a => a.ExamAttemptId == attemptId);
            var questionIds = answers.Select(a => a.QuestionId).ToList();

            var allQuestions = await _questionRepo.GetAllAsync();
            var attemptQuestions = allQuestions.Where(q => questionIds.Contains(q.Id)).ToList();

            return attemptQuestions.Select(q => new QuestionDto
            {
                Id = q.Id,
                ExamId = q.ExamId,
                Text = q.Text,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = ""
            });
        });

        public Task SaveAnswerAsync(int attemptId, int questionId, string selectedAnswer, bool isFlagged) => ExecuteWithLockAsync(async () =>
        {
            var answers = await _answerRepo.FindAsync(a => a.ExamAttemptId == attemptId && a.QuestionId == questionId);
            var answer = answers.FirstOrDefault();

            if (answer != null)
            {
                answer.SelectedAnswer = selectedAnswer;
                answer.IsFlagged = isFlagged;
                _answerRepo.Update(answer);
                await _answerRepo.SaveChangesAsync();
            }
        });

        public Task<ExamAttemptDto> SubmitExamAsync(int attemptId) => ExecuteWithLockAsync(async () =>
        {
            var attempt = await _attemptRepo.GetByIdAsync(attemptId);
            if (attempt == null)
                throw new Exception("Invalid attempt.");

            var exam = await _examRepo.GetByIdAsync(attempt.ExamId);
            var resultAnswers = await _answerRepo.FindAsync(a => a.ExamAttemptId == attemptId);

            if (attempt.IsSubmitted)
            {
                return new ExamAttemptDto
                {
                    Id = attempt.Id,
                    StudentId = attempt.StudentId,
                    ExamId = attempt.ExamId,
                    ExamTitle = exam?.Title ?? "",
                    StartTime = attempt.StartTime,
                    EndTime = attempt.EndTime,
                    Score = attempt.Score,
                    TotalQuestions = resultAnswers.Count(),
                    IsSubmitted = attempt.IsSubmitted,
                    Answers = resultAnswers.Select(a => new StudentAnswerDto
                    {
                        QuestionId = a.QuestionId,
                        SelectedAnswer = a.SelectedAnswer,
                        IsFlagged = a.IsFlagged
                    }).ToList()
                };
            }

            var answers = resultAnswers.ToList();
            var questions = (await _questionRepo.GetAllAsync()).Where(q => answers.Select(a => a.QuestionId).Contains(q.Id)).ToList();

            int score = 0;
            foreach (var ans in answers)
            {
                var q = questions.FirstOrDefault(x => x.Id == ans.QuestionId);
                if (q != null && !string.IsNullOrEmpty(ans.SelectedAnswer) && ans.SelectedAnswer == q.CorrectAnswer)
                {
                    score++;
                }
            }

            attempt.Score = score;
            attempt.EndTime = DateTime.UtcNow;
            attempt.IsSubmitted = true;

            _attemptRepo.Update(attempt);
            await _attemptRepo.SaveChangesAsync();

            return new ExamAttemptDto
            {
                Id = attempt.Id,
                StudentId = attempt.StudentId,
                ExamId = attempt.ExamId,
                ExamTitle = exam?.Title ?? "",
                StartTime = attempt.StartTime,
                EndTime = attempt.EndTime,
                Score = attempt.Score,
                TotalQuestions = resultAnswers.Count(),
                IsSubmitted = attempt.IsSubmitted,
                Answers = resultAnswers.Select(a => new StudentAnswerDto
                {
                    QuestionId = a.QuestionId,
                    SelectedAnswer = a.SelectedAnswer,
                    IsFlagged = a.IsFlagged
                }).ToList()
            };
        });

        public Task<ExamAttemptDto?> GetAttemptResultAsync(int attemptId) => ExecuteWithLockAsync(async () =>
        {
            var attempt = await _attemptRepo.GetByIdAsync(attemptId);
            if (attempt == null) return (ExamAttemptDto?)null;

            var exam = await _examRepo.GetByIdAsync(attempt.ExamId);
            var answers = await _answerRepo.FindAsync(a => a.ExamAttemptId == attemptId);

            return new ExamAttemptDto
            {
                Id = attempt.Id,
                StudentId = attempt.StudentId,
                ExamId = attempt.ExamId,
                ExamTitle = exam?.Title ?? "",
                StartTime = attempt.StartTime,
                EndTime = attempt.EndTime,
                Score = attempt.Score,
                TotalQuestions = 10,
                IsSubmitted = attempt.IsSubmitted,
                Answers = answers.Select(a => new StudentAnswerDto
                {
                    QuestionId = a.QuestionId,
                    SelectedAnswer = a.SelectedAnswer,
                    IsFlagged = a.IsFlagged
                }).ToList()
            };
        });

        public Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsAsync(string studentId) => ExecuteWithLockAsync(async () =>
        {
            var attempts = await _attemptRepo.FindAsync(a => a.StudentId == studentId);
            var exams = await _examRepo.GetAllAsync();

            return attempts.Select(a => 
            {
                var exam = exams.FirstOrDefault(e => e.Id == a.ExamId);
                return new ExamAttemptDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    ExamId = a.ExamId,
                    ExamTitle = exam?.Title ?? "",
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Score = a.Score,
                    TotalQuestions = exam?.TotalQuestions ?? 10,
                    IsSubmitted = a.IsSubmitted
                };
            });
        });
    }
}
