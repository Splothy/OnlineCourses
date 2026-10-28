using Application.Courses.Exceptions;
using LanguageExt;
using OnlineCourses.Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ILessonRepository
{
    Task<Option<Lesson>> GetById(CourseId courseId, LessonId lessonId, CancellationToken cancellationToken);
    Task<Option<Lesson>> GetByOrder(CourseId courseId, int order, CancellationToken cancellationToken);
    Task<Either<CourseException, Lesson>> Add(Lesson lesson, CancellationToken cancellationToken);
    Task<Either<CourseException, Lesson>> Update(Lesson lesson, CancellationToken cancellationToken);
}
