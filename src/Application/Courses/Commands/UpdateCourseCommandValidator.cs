using FluentValidation;

namespace Application.Courses.Commands;

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Level).NotEmpty().MaximumLength(40);
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
    }
}
