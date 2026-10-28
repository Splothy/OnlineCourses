using Application.Abstractions.Persistence;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Lessons.Queries;

public sealed record GetLessonByIdQuery(CourseId CourseId, LessonId LessonId) : IRequest<Option<Lesson>>;

public sealed class GetLessonByIdQueryHandler(ILessonQueries lessons)
    : IRequestHandler<GetLessonByIdQuery, Option<Lesson>>
{
    public Task<Option<Lesson>> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken) =>
        lessons.GetById(request.CourseId, request.LessonId, cancellationToken);
}
