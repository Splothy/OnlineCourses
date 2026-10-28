using OnlineCourses.Domain.Entities;

namespace Application.Courses.Exceptions;

public abstract class CourseException(CourseId courseId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public CourseId CourseId { get; } = courseId;
}

public sealed class CourseAlreadyExistsException(CourseId courseId)
    : CourseException(courseId, $"Course already exists under id {courseId}.");

public sealed class CourseNotFoundException(CourseId courseId)
    : CourseException(courseId, $"Course not found under id {courseId}.");

public sealed class CourseUnhandledException(CourseId courseId, Exception? innerException = null)
    : CourseException(courseId, $"Unhandled exception under id {courseId}.", innerException);

public sealed class LessonNotFoundException(CourseId courseId)
    : CourseException(courseId, $"Lesson not found under course {courseId}.");

public sealed class LessonAlreadyExistsException(CourseId courseId)
    : CourseException(courseId, $"A lesson with this order already exists in course {courseId}.");
