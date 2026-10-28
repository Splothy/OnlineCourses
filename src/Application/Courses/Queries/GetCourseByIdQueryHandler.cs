using Application.Abstractions.Persistence;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Queries;

public sealed class GetCourseByIdQueryHandler(ICourseQueries courses)
    : IRequestHandler<GetCourseByIdQuery, Option<Course>>
{
    public Task<Option<Course>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        return courses.GetById(request.Id, cancellationToken);
    }
}
