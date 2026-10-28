using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Commands;

public sealed record CreateCourseCommand(string Title, string Description, string Level, int DurationInMinutes)
    : IRequest<Either<CourseException, Course>>;

public sealed class CreateCourseCommandHandler(ICourseRepository courses)
    : IRequestHandler<CreateCourseCommand, Either<CourseException, Course>>
{
    public async Task<Either<CourseException, Course>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var existing = await courses.GetByTitle(request.Title.Trim(), cancellationToken);
        if (existing.IsSome)
            return new CourseAlreadyExistsException(existing.IfNone(() => throw new InvalidOperationException()).Id);

        return await Create(request, cancellationToken);
    }

    private async Task<Either<CourseException, Course>> Create(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = Course.Create(request.Title, request.Description, request.Level, request.DurationInMinutes);
            return await courses.Add(course, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(CourseId.Empty, exception);
        }
    }
}
