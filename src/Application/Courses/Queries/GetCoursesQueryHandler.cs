using Application.Abstractions.Persistence;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Queries;

public sealed class GetCoursesQueryHandler(ICourseQueries courses)
    : IRequestHandler<GetCoursesQuery, IReadOnlyList<Course>>
{
    public Task<IReadOnlyList<Course>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken)
    {
        return courses.GetAll(cancellationToken);
    }
}
