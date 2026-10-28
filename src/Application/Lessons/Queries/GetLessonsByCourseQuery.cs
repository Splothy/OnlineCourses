using Application.Abstractions.Persistence;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Lessons.Queries;

public sealed record GetLessonsByCourseQuery(CourseId CourseId) : IRequest<Option<IReadOnlyList<Lesson>>>;

public sealed class GetLessonsByCourseQueryHandler(ICourseQueries courses, ILessonQueries lessons)
    : IRequestHandler<GetLessonsByCourseQuery, Option<IReadOnlyList<Lesson>>>
{
    public async Task<Option<IReadOnlyList<Lesson>>> Handle(GetLessonsByCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await courses.GetById(request.CourseId, cancellationToken);
        if (course.IsNone)
            return Option<IReadOnlyList<Lesson>>.None;

        return Option<IReadOnlyList<Lesson>>.Some(
            await lessons.GetByCourseId(request.CourseId, cancellationToken));
    }
}
