namespace OnlineCourses.Domain.Entities;

public readonly record struct LessonId(Guid Value)
{
    public static LessonId New() => new(Guid.NewGuid());
    public static LessonId Empty => new(Guid.Empty);
    public static LessonId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
