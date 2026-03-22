using BlazorApp8.Domain.Entities;
using BlazorApp8.Infrastructure.Data;

namespace BlazorApp8.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static void SeedExamsAndQuestions(ApplicationDbContext context)
        {
            if (!context.Subjects.Any())
            {
                var subject1 = new Subject { Name = "C# Programming", Description = "Learn C# language fundamentals" };
                var subject2 = new Subject { Name = "Data Structures", Description = "Core programming algorithms" };

                context.Subjects.AddRange(subject1, subject2);
                context.SaveChanges();

                var exam1 = new Exam
                {
                    SubjectId = subject1.Id,
                    Title = "C# Basics",
                    DurationMinutes = 5,
                    TotalQuestions = 10
                };

                var exam2 = new Exam
                {
                    SubjectId = subject2.Id,
                    Title = "Common Algorithms",
                    DurationMinutes = 5,
                    TotalQuestions = 10
                };

                context.Exams.AddRange(exam1, exam2);
                context.SaveChanges();

                // Add 30+ questions for exam 1 (.NET Core)
                var netCoreQuestions = new List<Question>
                {
                    new Question { ExamId = exam1.Id, Text = "What does CLR stand for in .NET?", OptionA = "Common Language Runtime", OptionB = "C-Level Runtime", OptionC = "Compile Logic Reference", OptionD = "Common Linear Runtime", CorrectAnswer = "A" },
                    new Question { ExamId = exam1.Id, Text = "Which keyword is used to inherit a class in C#?", OptionA = "extends", OptionB = "implements", OptionC = ": (colon)", OptionD = "inherits", CorrectAnswer = "C" },
                    new Question { ExamId = exam1.Id, Text = "What is the entry point of a C# console application?", OptionA = "Start()", OptionB = "Init()", OptionC = "Run()", OptionD = "Main()", CorrectAnswer = "D" },
                    new Question { ExamId = exam1.Id, Text = "Which of the following is a value type in C#?", OptionA = "string", OptionB = "int", OptionC = "class", OptionD = "interface", CorrectAnswer = "B" },
                    new Question { ExamId = exam1.Id, Text = "How do you declare a constant variable in C#?", OptionA = "const", OptionB = "final", OptionC = "static", OptionD = "readonly", CorrectAnswer = "A" },
                    new Question { ExamId = exam1.Id, Text = "Which LINQ method is used to filter a collection?", OptionA = "Select()", OptionB = "Where()", OptionC = "OrderBy()", OptionD = "Filter()", CorrectAnswer = "B" },
                    new Question { ExamId = exam1.Id, Text = "What does the 'await' keyword do?", OptionA = "Blocks the thread permanently", OptionB = "Creates a new thread", OptionC = "Asynchronously waits for a task to complete", OptionD = "Immediately terminates the method", CorrectAnswer = "C" },
                    new Question { ExamId = exam1.Id, Text = "Which collection type stores key-value pairs?", OptionA = "List<T>", OptionB = "Array", OptionC = "Queue<T>", OptionD = "Dictionary<TKey, TValue>", CorrectAnswer = "D" },
                    new Question { ExamId = exam1.Id, Text = "What is 'Blazor'?", OptionA = "A database engine", OptionB = "A UI framework using C# and WebAssembly", OptionC = "A CSS library", OptionD = "A cloud hosting provider", CorrectAnswer = "B" },
                    new Question { ExamId = exam1.Id, Text = "Which access modifier restricts visibility to the current class?", OptionA = "public", OptionB = "protected", OptionC = "private", OptionD = "internal", CorrectAnswer = "C" }
                };
                context.Questions.AddRange(netCoreQuestions);

                var dataStructureQuestions = new List<Question>
                {
                    new Question { ExamId = exam2.Id, Text = "Which data structure uses LIFO (Last In First Out)?", OptionA = "Queue", OptionB = "Stack", OptionC = "Tree", OptionD = "Graph", CorrectAnswer = "B" },
                    new Question { ExamId = exam2.Id, Text = "What is the time complexity of searching in a Hash Table (average case)?", OptionA = "O(1)", OptionB = "O(N)", OptionC = "O(log N)", OptionD = "O(N^2)", CorrectAnswer = "A" },
                    new Question { ExamId = exam2.Id, Text = "Which data structure organizes items in a hierarchy?", OptionA = "Array", OptionB = "Linked List", OptionC = "Tree", OptionD = "Queue", CorrectAnswer = "C" },
                    new Question { ExamId = exam2.Id, Text = "What is the maximum number of children a binary tree node can have?", OptionA = "0", OptionB = "1", OptionC = "2", OptionD = "Unlimited", CorrectAnswer = "C" },
                    new Question { ExamId = exam2.Id, Text = "Which sorting algorithm is typically generally the fastest on average?", OptionA = "Bubble Sort", OptionB = "Quick Sort", OptionC = "Insertion Sort", OptionD = "Quick/Merge Sort", CorrectAnswer = "D" },
                    new Question { ExamId = exam2.Id, Text = "What follows FIFO (First In First Out)?", OptionA = "Stack", OptionB = "Queue", OptionC = "Tree", OptionD = "Heap", CorrectAnswer = "B" },
                    new Question { ExamId = exam2.Id, Text = "A linked list node typically contains data and...?", OptionA = "A pointer to the next node", OptionB = "The total list size", OptionC = "A hash code", OptionD = "An array index", CorrectAnswer = "A" },
                    new Question { ExamId = exam2.Id, Text = "Which of the following is not a linear data structure?", OptionA = "Array", OptionB = "Linked List", OptionC = "Graph", OptionD = "Stack", CorrectAnswer = "C" },
                    new Question { ExamId = exam2.Id, Text = "What algorithm is used to find the shortest path between graph nodes?", OptionA = "Dijkstra's Algorithm", OptionB = "Binary Search", OptionC = "Merge Sort", OptionD = "K-Means", CorrectAnswer = "A" },
                    new Question { ExamId = exam2.Id, Text = "In a standard Binary Search Tree, where are smaller values placed relative to the root?", OptionA = "To the right", OptionB = "To the left", OptionC = "Above", OptionD = "Randomly", CorrectAnswer = "B" }
                };
                context.Questions.AddRange(dataStructureQuestions);

                context.SaveChanges();
            }
        }
    }
}
