namespace Api.Dtos;

public sealed record CourseResponse(
    Guid Id,
    string Title,
    string Description,
    string Level,
    int DurationInMinutes,
    DateTime CreatedAt,
    IReadOnlyList<LessonResponse> Lessons);

public sealed record LessonResponse(
    Guid Id,
    string Title,
    int Order,
    string ContentType,
    DateTime CreatedAt);

public sealed record CreateCourseRequest(
    string Title,
    string Description,
    string Level,
    int DurationInMinutes);

public sealed record UpdateCourseRequest(
    string Title,
    string Description,
    string Level,
    int DurationInMinutes);

public sealed record CreateLessonRequest(string Title, int Order, string ContentType);

public sealed record UpdateLessonRequest(string Title, int Order, string ContentType);
