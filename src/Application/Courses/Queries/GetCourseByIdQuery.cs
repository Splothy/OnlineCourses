using LanguageExt;
using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Queries;

public sealed record GetCourseByIdQuery(CourseId Id)
    : IRequest<Option<Course>>;
