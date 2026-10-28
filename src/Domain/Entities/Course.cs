namespace OnlineCourses.Domain.Entities;

public sealed class Course
{
    private readonly List<Lesson> _lessons = [];

    private Course() { }

    private Course(string title, string description, string level, int durationInMinutes)
    {
        Id = CourseId.New();
        CreatedAt = DateTime.UtcNow;
        UpdateDetails(title, description, level, durationInMinutes);
    }

    public CourseId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    public int DurationInMinutes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    public static Course Create(string title, string description, string level, int durationInMinutes) =>
        new(title, description, level, durationInMinutes);

    public void UpdateDetails(string title, string description, string level, int durationInMinutes)
    {
        Title = Required(title, nameof(title), 160);
        Description = Required(description, nameof(description), 4000);
        Level = Required(level, nameof(level), 40);
        if (durationInMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationInMinutes), "Duration must be greater than zero.");

        DurationInMinutes = durationInMinutes;
    }

    public void AddLesson(Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        if (_lessons.Any(existing => existing.Id == lesson.Id))
            throw new InvalidOperationException("This lesson is already part of the course.");
        if (_lessons.Any(existing => existing.Order == lesson.Order))
            throw new InvalidOperationException("A lesson with this order already exists in the course.");

        lesson.AssignToCourse(Id);
        _lessons.Add(lesson);
    }

    public bool RemoveLesson(LessonId lessonId)
    {
        var lesson = _lessons.SingleOrDefault(item => item.Id == lessonId);
        return lesson is not null && _lessons.Remove(lesson);
    }

    private static string Required(string value, string parameter, int maxLength)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameter);
        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameter);
        return normalized;
    }
}
