using Application.Courses.Exceptions;
using LanguageExt;
using OnlineCourses.Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ICourseRepository
{
    Task<Option<Course>> GetById(CourseId id, CancellationToken cancellationToken);
    Task<Option<Course>> GetByTitle(string title, CancellationToken cancellationToken);
    Task<Either<CourseException, Course>> Add(Course course, CancellationToken cancellationToken);
    Task<Either<CourseException, Course>> Update(Course course, CancellationToken cancellationToken);
}
