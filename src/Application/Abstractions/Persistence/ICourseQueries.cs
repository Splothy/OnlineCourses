using LanguageExt;
using OnlineCourses.Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ICourseQueries
{
    Task<IReadOnlyList<Course>> GetAll(CancellationToken cancellationToken);
    Task<Option<Course>> GetById(CourseId id, CancellationToken cancellationToken);
}
