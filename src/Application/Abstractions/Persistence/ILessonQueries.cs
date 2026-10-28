using LanguageExt;
using OnlineCourses.Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ILessonQueries
{
    Task<IReadOnlyList<Lesson>> GetByCourseId(CourseId courseId, CancellationToken cancellationToken);
    Task<Option<Lesson>> GetById(CourseId courseId, LessonId lessonId, CancellationToken cancellationToken);
}
