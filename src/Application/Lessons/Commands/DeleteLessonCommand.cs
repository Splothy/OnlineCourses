using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Lessons.Commands;

public sealed record DeleteLessonCommand(CourseId CourseId, LessonId LessonId)
    : IRequest<Either<CourseException, Course>>;

public sealed class DeleteLessonCommandHandler(ICourseRepository courses)
    : IRequestHandler<DeleteLessonCommand, Either<CourseException, Course>>
{
    public async Task<Either<CourseException, Course>> Handle(
        DeleteLessonCommand request,
        CancellationToken cancellationToken)
    {
        var courseOption = await courses.GetById(request.CourseId, cancellationToken);
        if (courseOption.IsNone)
            return new CourseNotFoundException(request.CourseId);

        var course = courseOption.IfNone(() => throw new InvalidOperationException());
        if (!course.RemoveLesson(request.LessonId))
            return new LessonNotFoundException(request.CourseId);

        try
        {
            return await courses.Update(course, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(request.CourseId, exception);
        }
    }
}
