namespace OnlineCourses.Domain.Entities;

public readonly record struct CourseId(Guid Value)
{
    public static CourseId New() => new(Guid.NewGuid());
    public static CourseId Empty => new(Guid.Empty);
    public static CourseId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
