using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Commands;

public sealed record UpdateCourseCommand(CourseId CourseId, string Title, string Description, string Level, int DurationInMinutes)
    : IRequest<Either<CourseException, Course>>;

public sealed class UpdateCourseCommandHandler(ICourseRepository courses)
    : IRequestHandler<UpdateCourseCommand, Either<CourseException, Course>>
{
    public async Task<Either<CourseException, Course>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await courses.GetById(request.CourseId, cancellationToken);
        if (course.IsNone)
            return new CourseNotFoundException(request.CourseId);

        var entity = course.IfNone(() => throw new InvalidOperationException());
        var duplicateError = await CheckDuplicate(entity, request, cancellationToken);
        if (duplicateError is not null)
            return duplicateError;
        return await Update(entity, request, cancellationToken);
    }

    private async Task<CourseException?> CheckDuplicate(Course current, UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var existing = await courses.GetByTitle(request.Title.Trim(), cancellationToken);
        if (existing.IsNone)
            return null;
        var other = existing.IfNone(() => throw new InvalidOperationException());
        return other.Id == current.Id ? null : new CourseAlreadyExistsException(other.Id);
    }

    private async Task<Either<CourseException, Course>> Update(Course course, UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            course.UpdateDetails(request.Title, request.Description, request.Level, request.DurationInMinutes);
            return await courses.Update(course, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(course.Id, exception);
        }
    }
}
