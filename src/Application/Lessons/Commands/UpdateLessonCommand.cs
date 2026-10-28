using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Lessons.Commands;

public sealed record UpdateLessonCommand(CourseId CourseId, LessonId LessonId, string Title, int Order, string ContentType)
    : IRequest<Either<CourseException, Lesson>>;

public sealed class UpdateLessonCommandHandler(ILessonRepository lessons)
    : IRequestHandler<UpdateLessonCommand, Either<CourseException, Lesson>>
{
    public async Task<Either<CourseException, Lesson>> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lessonOption = await lessons.GetById(request.CourseId, request.LessonId, cancellationToken);
        if (lessonOption.IsNone)
            return new LessonNotFoundException(request.CourseId);

        var existingAtOrder = await lessons.GetByOrder(request.CourseId, request.Order, cancellationToken);
        if (existingAtOrder.IsSome && existingAtOrder.IfNone(() => throw new InvalidOperationException()).Id != request.LessonId)
            return new LessonAlreadyExistsException(request.CourseId);

        var lesson = lessonOption.IfNone(() => throw new InvalidOperationException());
        try
        {
            lesson.UpdateDetails(request.Title, request.Order, request.ContentType);
            return await lessons.Update(lesson, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(request.CourseId, exception);
        }
    }
}
