using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Lessons.Commands;

public sealed record CreateLessonCommand(CourseId CourseId, string Title, int Order, string ContentType)
    : IRequest<Either<CourseException, Lesson>>;

public sealed class CreateLessonCommandHandler(ICourseRepository courses, ILessonRepository lessons)
    : IRequestHandler<CreateLessonCommand, Either<CourseException, Lesson>>
{
    public async Task<Either<CourseException, Lesson>> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var courseOption = await courses.GetById(request.CourseId, cancellationToken);
        if (courseOption.IsNone)
            return new CourseNotFoundException(request.CourseId);

        var existing = await lessons.GetByOrder(request.CourseId, request.Order, cancellationToken);
        if (existing.IsSome)
            return new LessonAlreadyExistsException(request.CourseId);

        try
        {
            var course = courseOption.IfNone(() => throw new InvalidOperationException());
            var lesson = Lesson.Create(request.Title, request.Order, request.ContentType);
            course.AddLesson(lesson);
            return await lessons.Add(lesson, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(request.CourseId, exception);
        }
    }
}
