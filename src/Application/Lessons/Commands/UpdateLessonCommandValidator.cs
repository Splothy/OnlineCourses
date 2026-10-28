using FluentValidation;

namespace Application.Lessons.Commands;

public sealed class UpdateLessonCommandValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(command => command.CourseId).NotEmpty();
        RuleFor(command => command.LessonId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(160);
        RuleFor(command => command.Order).GreaterThan(0);
        RuleFor(command => command.ContentType).NotEmpty().MaximumLength(40);
    }
}
