using FluentValidation;

namespace Application.Lessons.Commands;

public sealed class DeleteLessonCommandValidator : AbstractValidator<DeleteLessonCommand>
{
    public DeleteLessonCommandValidator()
    {
        RuleFor(command => command.CourseId).NotEmpty();
        RuleFor(command => command.LessonId).NotEmpty();
    }
}
