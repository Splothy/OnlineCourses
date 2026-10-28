using FluentValidation;

namespace Application.Courses.Commands;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Level).NotEmpty().MaximumLength(40);
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
    }
}
