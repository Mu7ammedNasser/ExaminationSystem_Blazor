namespace BlazorApp8.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}
