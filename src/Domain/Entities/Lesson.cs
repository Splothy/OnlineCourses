namespace OnlineCourses.Domain.Entities;

public sealed class Lesson
{
    private Lesson() { }

    private Lesson(string title, int order, string contentType)
    {
        Id = LessonId.New();
        CreatedAt = DateTime.UtcNow;
        UpdateDetails(title, order, contentType);
    }

    public LessonId Id { get; private set; }
    public CourseId CourseId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static Lesson Create(string title, int order, string contentType) =>
        new(title, order, contentType);

    public void UpdateDetails(string title, int order, string contentType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        var normalizedTitle = title.Trim();
        var normalizedType = contentType.Trim();
        if (normalizedTitle.Length > 160)
            throw new ArgumentException("Title cannot exceed 160 characters.", nameof(title));
        if (normalizedType.Length > 40)
            throw new ArgumentException("Content type cannot exceed 40 characters.", nameof(contentType));
        if (order <= 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Lesson order must be greater than zero.");

        Title = normalizedTitle;
        Order = order;
        ContentType = normalizedType;
    }

    internal void AssignToCourse(CourseId courseId)
    {
        if (CourseId != default && CourseId != courseId)
            throw new InvalidOperationException("A lesson cannot belong to more than one course.");
        CourseId = courseId;
    }
}
